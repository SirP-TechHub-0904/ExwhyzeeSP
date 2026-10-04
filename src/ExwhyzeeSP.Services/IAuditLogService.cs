using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Services
{
    public interface IAuditLogService
    {
        Task<MasterAuditLog> LogAsync(
            string action,
            string details,
            string? userName = null,
            string? userRole = null,
            string area = "Master",
            string severity = "Info",
            string? tenantSubDomain = null,
            int? tenantSchoolId = null,
            HttpContext? httpContext = null);
    }
}
