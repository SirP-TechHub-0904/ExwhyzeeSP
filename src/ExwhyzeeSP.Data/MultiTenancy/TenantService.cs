using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Data.MultiTenancy
{
    /// <summary>
    /// Thread-safe scoped tenant service with in-memory caching.
    /// Fast and robust host/subdomain resolution for every incoming HTTP request.
    /// </summary>
    public class TenantService : ITenantService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly MasterDbContext _masterDbContext;
        private TenantSchool? _currentTenant;

        public TenantService(
            IMemoryCache cache,
            IConfiguration configuration,
            MasterDbContext masterDbContext)
        {
            _cache = cache;
            _configuration = configuration;
            _masterDbContext = masterDbContext;
        }

        public TenantSchool? GetCurrentTenant() => _currentTenant;

        public void SetCurrentTenant(TenantSchool tenant)
        {
            _currentTenant = tenant;
        }

        public string GetActiveConnectionString()
        {
            if (_currentTenant != null && !string.IsNullOrWhiteSpace(_currentTenant.ConnectionString))
            {
                return _currentTenant.ConnectionString;
            }

            return string.Empty;
        }

        public async Task<TenantSchool?> ResolveTenantByHostAsync(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                return null;

            // Strip port if present (e.g. "divinethreshers.exwhyzee.ng:5000" -> "divinethreshers.exwhyzee.ng")
            var cleanedHost = host.Split(':')[0].Trim().ToLowerInvariant();

            var cacheKey = $"tenant_host_{cleanedHost}";
            if (_cache.TryGetValue(cacheKey, out TenantSchool? cachedTenant))
            {
                return cachedTenant;
            }


            // Extract potential subdomain (e.g. "mercygirls.exwhyzeesp.com" -> "mercygirls")
            var hostParts = cleanedHost.Split('.');
            string? potentialSubdomain = hostParts.Length > 2 ? hostParts[0] : null;

            // Query Master Database safely with fallback protection
            TenantSchool? tenant = null;
            try
            {
                tenant = await _masterDbContext.TenantSchools
                    .AsNoTracking()
                    .Where(t => t.IsActive && (
                        (t.HostDomain != null && t.HostDomain.ToLower() == cleanedHost) ||
                        (potentialSubdomain != null && t.SubDomain.ToLower() == potentialSubdomain) ||
                        t.SubDomain.ToLower() == cleanedHost
                    ))
                    .FirstOrDefaultAsync();
            }
            catch
            {
                // If master DB is not reachable or tables not yet created, return null to fallback smoothly
                tenant = null;
            }

            // Cache for 15 minutes
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            _cache.Set(cacheKey, tenant, cacheOptions);
            return tenant;
        }
    }
}
