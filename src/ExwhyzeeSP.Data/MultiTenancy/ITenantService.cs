using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Data.MultiTenancy
{
    /// <summary>
    /// Contract for resolving and retrieving the current tenant school in the current HTTP request.
    /// </summary>
    public interface ITenantService
    {
        /// <summary>
        /// Gets the current resolved tenant for the active HTTP request.
        /// Returns null if on root master domain or fallback.
        /// </summary>
        TenantSchool? GetCurrentTenant();

        /// <summary>
        /// Gets the active database connection string for the current request.
        /// If tenant is resolved, returns school's connection string; otherwise returns default fallback connection string.
        /// </summary>
        string GetActiveConnectionString();

        /// <summary>
        /// Sets the current tenant for the active request scope.
        /// </summary>
        void SetCurrentTenant(TenantSchool tenant);

        /// <summary>
        /// Resolves tenant from hostname asynchronously.
        /// </summary>
        Task<TenantSchool?> ResolveTenantByHostAsync(string host);
    }
}
