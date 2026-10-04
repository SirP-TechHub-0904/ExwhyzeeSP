using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly MasterDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(MasterDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MasterAuditLog> LogAsync(
            string action,
            string details,
            string? userName = null,
            string? userRole = null,
            string area = "Master",
            string severity = "Info",
            string? tenantSubDomain = null,
            int? tenantSchoolId = null,
            HttpContext? httpContext = null)
        {
            var ctx = httpContext ?? _httpContextAccessor.HttpContext;
            var ip = GetClientIpAddress(ctx);
            var userAgent = ctx?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";

            var (browser, os, device) = ParseUserAgent(userAgent);

            var resolvedUserName = !string.IsNullOrWhiteSpace(userName)
                ? userName
                : (ctx?.User?.Identity?.IsAuthenticated == true
                    ? ctx.User.Identity.Name
                    : "Anonymous");

            var log = new MasterAuditLog
            {
                TimestampUtc = DateTime.UtcNow,
                UserName = resolvedUserName ?? "Anonymous",
                UserRole = userRole,
                Action = action,
                Details = details,
                Area = area,
                Severity = severity,
                IpAddress = ip,
                UserAgent = userAgent.Length > 500 ? userAgent.Substring(0, 497) + "..." : userAgent,
                Browser = browser,
                OperatingSystem = os,
                DeviceType = device,
                TenantSubDomain = tenantSubDomain,
                TenantSchoolId = tenantSchoolId
            };

            try
            {
                await _context.AuditLogs.AddAsync(log);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Safety guard: failure to write an audit log should never crash the main application thread
            }

            return log;
        }

        private static string GetClientIpAddress(HttpContext? context)
        {
            if (context == null) return "127.0.0.1";

            var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                var ips = forwardedFor.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (ips.Length > 0)
                {
                    return ips[0].Trim();
                }
            }

            var realIp = context.Request.Headers["X-Real-IP"].ToString();
            if (!string.IsNullOrWhiteSpace(realIp))
            {
                return realIp.Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        }

        private static (string Browser, string OS, string Device) ParseUserAgent(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent) || userAgent == "Unknown")
            {
                return ("Unknown", "Unknown", "Desktop");
            }

            // 1. Device Type
            string device = "Desktop";
            if (Regex.IsMatch(userAgent, "(iPad|Tablet|PlayBook|Silk)", RegexOptions.IgnoreCase))
            {
                device = "Tablet";
            }
            else if (Regex.IsMatch(userAgent, "(Mobile|Android|iPhone|iPod|BlackBerry|IEMobile|Opera Mini)", RegexOptions.IgnoreCase))
            {
                device = "Mobile";
            }
            else if (Regex.IsMatch(userAgent, "(bot|crawler|spider|curl|wget)", RegexOptions.IgnoreCase))
            {
                device = "Bot";
            }

            // 2. Operating System
            string os = "Other";
            if (userAgent.Contains("Windows NT 10.0")) os = "Windows 10/11";
            else if (userAgent.Contains("Windows NT 6.3")) os = "Windows 8.1";
            else if (userAgent.Contains("Windows NT 6.1")) os = "Windows 7";
            else if (userAgent.Contains("Windows")) os = "Windows";
            else if (userAgent.Contains("Android")) os = "Android";
            else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad") || userAgent.Contains("iOS")) os = "iOS";
            else if (userAgent.Contains("Mac OS X") || userAgent.Contains("Macintosh")) os = "macOS";
            else if (userAgent.Contains("Linux")) os = "Linux";

            // 3. Browser
            string browser = "Browser";
            if (userAgent.Contains("Edg/")) browser = "Microsoft Edge";
            else if (userAgent.Contains("OPR/") || userAgent.Contains("Opera")) browser = "Opera";
            else if (userAgent.Contains("Chrome/") && !userAgent.Contains("Edg/")) browser = "Google Chrome";
            else if (userAgent.Contains("Safari/") && !userAgent.Contains("Chrome/")) browser = "Apple Safari";
            else if (userAgent.Contains("Firefox/")) browser = "Mozilla Firefox";
            else if (userAgent.Contains("SamsungBrowser")) browser = "Samsung Internet";

            return (browser, os, device);
        }
    }
}
