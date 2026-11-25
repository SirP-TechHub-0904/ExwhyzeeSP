using System.Threading;
using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Data.MultiTenancy
{
    /// <summary>
    /// Thread-safe ambient context that holds the active tenant for the current asynchronous execution flow.
    /// Ensures that legacy controllers, views, and services instantiating 'new ApplicationDbContext()'
    /// automatically connect to the correct school tenant database resolved for the current HTTP request.
    /// </summary>
    public static class TenantContext
    {
        private static readonly AsyncLocal<TenantSchool?> _currentTenant = new();

        public static TenantSchool? CurrentTenant
        {
            get => _currentTenant.Value;
            set => _currentTenant.Value = value;
        }

        public static string? CurrentConnectionString => _currentTenant.Value?.ConnectionString;
    }
}
