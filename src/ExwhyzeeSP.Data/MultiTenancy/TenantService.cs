using System;
using System.Collections.Generic;
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
    /// Resolves tenants first from .env / Configuration, then falls back to MasterDbContext, then DEFAULT_DB_CONNECTION.
    /// </summary>
    public class TenantService : ITenantService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly MasterDbContext _masterDbContext;
        private TenantSchool? _currentTenant;

        private static readonly List<TenantSchool> _envTenants = new();
        private static bool _envTenantsLoaded = false;
        private static readonly object _envLock = new();

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

            return _configuration["DEFAULT_DB_CONNECTION"]
                ?? Environment.GetEnvironmentVariable("DEFAULT_DB_CONNECTION")
                ?? string.Empty;
        }

        private List<TenantSchool> GetEnvTenants()
        {
            if (_envTenantsLoaded) return _envTenants;
            lock (_envLock)
            {
                if (_envTenantsLoaded) return _envTenants;
                _envTenants.Clear();

                for (int i = 1; i <= 20; i++)
                {
                    var prefix = $"TENANT_{i}_";
                    var conn = _configuration[$"{prefix}CONNECTION"] 
                        ?? Environment.GetEnvironmentVariable($"{prefix}CONNECTION");

                    if (string.IsNullOrWhiteSpace(conn)) continue;

                    var name = _configuration[$"{prefix}NAME"] 
                        ?? Environment.GetEnvironmentVariable($"{prefix}NAME") 
                        ?? $"Tenant {i}";

                    var domains = _configuration[$"{prefix}DOMAINS"] 
                        ?? Environment.GetEnvironmentVariable($"{prefix}DOMAINS") 
                        ?? string.Empty;

                    var subdomain = _configuration[$"{prefix}SUBDOMAIN"] 
                        ?? Environment.GetEnvironmentVariable($"{prefix}SUBDOMAIN") 
                        ?? string.Empty;

                    _envTenants.Add(new TenantSchool
                    {
                        Id = i,
                        SchoolName = name,
                        SubDomain = subdomain,
                        HostDomain = domains,
                        ConnectionString = conn,
                        IsActive = true
                    });
                }
                _envTenantsLoaded = true;
            }
            return _envTenants;
        }

        public async Task<TenantSchool?> ResolveTenantByHostAsync(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                return GetDefaultFallbackTenant();

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

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            // 1. Resolve from .env defined tenants (Fast in-memory match, 0 DB latency)
            var envTenants = GetEnvTenants();
            if (envTenants.Any())
            {
                var envMatched = envTenants.FirstOrDefault(t =>
                {
                    var domainList = (t.HostDomain ?? "")
                        .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(d => d.ToLowerInvariant())
                        .ToList();

                    if (domainList.Contains(cleanedHost)) return true;
                    if (!string.IsNullOrWhiteSpace(t.SubDomain) && 
                        (t.SubDomain.ToLowerInvariant() == cleanedHost || t.SubDomain.ToLowerInvariant() == potentialSubdomain))
                    {
                        return true;
                    }

                    return false;
                });

                if (envMatched != null)
                {
                    _cache.Set(cacheKey, envMatched, cacheOptions);
                    return envMatched;
                }
            }

            // 2. Query Master Database safely with fallback protection
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
                tenant = null;
            }

            // 3. Fallback to default tenant (Divine Threshers safe default)
            tenant ??= GetDefaultFallbackTenant();

            if (tenant != null)
            {
                _cache.Set(cacheKey, tenant, cacheOptions);
            }

            return tenant;
        }

        private TenantSchool? GetDefaultFallbackTenant()
        {
            var defaultConn = _configuration["DEFAULT_DB_CONNECTION"]
                ?? Environment.GetEnvironmentVariable("DEFAULT_DB_CONNECTION");

            if (string.IsNullOrWhiteSpace(defaultConn))
                return null;

            var envTenants = GetEnvTenants();
            var firstTenant = envTenants.FirstOrDefault();

            return new TenantSchool
            {
                Id = firstTenant?.Id ?? 1,
                SchoolName = firstTenant?.SchoolName ?? "Default School Tenant",
                SubDomain = firstTenant?.SubDomain ?? "default",
                HostDomain = firstTenant?.HostDomain ?? "default",
                ConnectionString = defaultConn,
                IsActive = true
            };
        }
    }
}
