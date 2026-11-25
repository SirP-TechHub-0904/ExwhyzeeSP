using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace ExwhyzeeSP.Data.MultiTenancy
{
    /// <summary>
    /// Middleware that executes at the start of every HTTP request.
    /// Intercepts the request hostname/subdomain, resolves the tenant school,
    /// and populates the scoped ITenantService and HttpContext.Items.
    /// </summary>
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

            // Allow static files, health checks, and favicons to pass through
            if (path.StartsWith("/layout/") ||
                path.StartsWith("/css/") ||
                path.StartsWith("/js/") ||
                path.StartsWith("/images/") ||
                path.StartsWith("/lib/") ||
                path.StartsWith("/favicon.ico") ||
                path == "/health")
            {
                await _next(context);
                return;
            }

            var host = context.Request.Headers["X-Forwarded-Host"].FirstOrDefault()
                ?? context.Request.Host.Host;

            if (host.Contains(":"))
            {
                host = host.Split(':')[0].Trim();
            }

            // Resolve tenant asynchronously from .env / Master DB
            var tenant = await tenantService.ResolveTenantByHostAsync(host);

            if (tenant != null && !string.IsNullOrWhiteSpace(tenant.ConnectionString))
            {
                // 1. FULL SUSPENSION or INACTIVE: Complete lockout of public site and portal
                if (!tenant.IsActive || tenant.SuspensionMode == Core.Models.TenantSuspensionMode.FullSuspension)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var suspensionHtml = GenerateSuspensionHtml(tenant, host, isPortalOnly: false);
                    await context.Response.WriteAsync(suspensionHtml);
                    return;
                }

                // 2. PORTAL & DASHBOARD SUSPENSION ONLY: Public website stays open, but logins/panels are blocked
                if (tenant.SuspensionMode == Core.Models.TenantSuspensionMode.PortalDashboardSuspensionOnly)
                {
                    var isPortalRoute = path.StartsWith("/account/login") ||
                                        path.StartsWith("/admin") ||
                                        path.StartsWith("/staff") ||
                                        path.StartsWith("/student") ||
                                        path.StartsWith("/financial") ||
                                        path.StartsWith("/superuser") ||
                                        path.StartsWith("/portal");

                    if (isPortalRoute)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "text/html; charset=utf-8";
                        var portalLockHtml = GenerateSuspensionHtml(tenant, host, isPortalOnly: true);
                        await context.Response.WriteAsync(portalLockHtml);
                        return;
                    }
                }

                // 3. MAINTENANCE MODE
                if (tenant.SuspensionMode == Core.Models.TenantSuspensionMode.MaintenanceMode && !path.StartsWith("/admin"))
                {
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var maintHtml = GenerateMaintenanceHtml(tenant, host);
                    await context.Response.WriteAsync(maintHtml);
                    return;
                }

                tenantService.SetCurrentTenant(tenant);
                TenantContext.CurrentTenant = tenant;
                context.Items["CurrentTenant"] = tenant;
                context.Items["TenantSchoolName"] = tenant.SchoolName;
                context.Items["TenantSubDomain"] = tenant.SubDomain;
                context.Items["TenantHostDomain"] = tenant.HostDomain;

                await _next(context);
                return;
            }

            // Allow default fallback if DEFAULT_DB_CONNECTION exists
            var fallbackConn = Environment.GetEnvironmentVariable("DEFAULT_DB_CONNECTION");
            if (!string.IsNullOrWhiteSpace(fallbackConn))
            {
                await _next(context);
                return;
            }

            // Tenant NOT found -> Return clean, friendly branded error page
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "text/html; charset=utf-8";

            var html = GenerateTenantNotFoundHtml(host);
            await context.Response.WriteAsync(html);
        }

        private static string GenerateSuspensionHtml(Core.Models.TenantSchool tenant, string host, bool isPortalOnly)
        {
            var title = isPortalOnly ? "School Portal Access Restricted" : "Institution Access Suspended";
            var heading = isPortalOnly ? $"{tenant.SchoolName} Portal Restricted" : $"{tenant.SchoolName} Inactive";
            var notice = !string.IsNullOrWhiteSpace(tenant.SuspensionNotice)
                ? tenant.SuspensionNotice
                : (isPortalOnly
                    ? "Administrative and student portal dashboards are temporarily locked for this institution. The public website remains accessible."
                    : $"The platform services for <strong>{host}</strong> are currently deactivated.");

            return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>{title} - Exwhyzee Platform</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap"" rel=""stylesheet"">
    <style>
        * {{ box-sizing: border-box; margin: 0; padding: 0; }}
        body {{
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: linear-gradient(135deg, #0f172a 0%, #1e1b4b 50%, #0f172a 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
            color: #f8fafc;
        }}
        .card {{
            background: rgba(30, 41, 59, 0.75);
            backdrop-filter: blur(16px);
            border: 1px solid rgba(239, 68, 68, 0.25);
            border-radius: 24px;
            padding: 48px;
            max-width: 600px;
            width: 100%;
            text-align: center;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.6);
        }}
        .icon-box {{
            width: 80px;
            height: 80px;
            border-radius: 20px;
            background: rgba(239, 68, 68, 0.15);
            border: 1px solid rgba(239, 68, 68, 0.35);
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 24px;
        }}
        h1 {{ font-size: 24px; font-weight: 800; margin-bottom: 12px; color: #ffffff; }}
        p {{ font-size: 15px; line-height: 1.6; color: #cbd5e1; margin-bottom: 24px; }}
        .notice-box {{
            background: rgba(15, 23, 42, 0.8);
            border-left: 4px solid #ef4444;
            padding: 16px;
            border-radius: 8px;
            text-align: left;
            font-size: 14px;
            color: #fca5a5;
            margin-bottom: 24px;
        }}
        .badge {{
            display: inline-block;
            background: #0f172a;
            border: 1px solid #334155;
            padding: 8px 16px;
            border-radius: 12px;
            font-family: monospace;
            font-size: 13px;
            color: #38bdf8;
            margin-bottom: 28px;
        }}
        .btn {{
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 12px 24px;
            border-radius: 12px;
            font-size: 14px;
            font-weight: 600;
            text-decoration: none;
            background: #2563eb;
            color: #ffffff;
            transition: all 0.2s;
        }}
        .btn:hover {{ background: #1d4ed8; }}
        .btn-outline {{
            background: rgba(255,255,255,0.05);
            border: 1px solid rgba(255,255,255,0.15);
            color: #f8fafc;
            margin-left: 10px;
        }}
    </style>
</head>
<body>
    <div class=""card"">
        <div class=""icon-box"">
            <svg width=""40"" height=""40"" fill=""none"" stroke=""#f87171"" stroke-width=""2"" viewBox=""0 0 24 24"">
                <path stroke-linecap=""round"" stroke-linejoin=""round"" d=""M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"" />
            </svg>
        </div>
        <h1>{heading}</h1>
        <div class=""badge"">{host}</div>
        <div class=""notice-box"">
            <strong>Notice:</strong> {notice}
        </div>
        <div class=""actions"">
            <a href=""mailto:bursary@{host}"" class=""btn"">Contact Bursar / Admin</a>
            <a href=""/"" class=""btn btn-outline"">Visit Public Website</a>
        </div>
    </div>
</body>
</html>";
        }

        private static string GenerateMaintenanceHtml(Core.Models.TenantSchool tenant, string host)
        {
            return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <title>Scheduled Maintenance - {tenant.SchoolName}</title>
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@400;600;800&display=swap"" rel=""stylesheet"">
    <style>
        body {{ font-family: 'Inter', sans-serif; background: #0f172a; color: white; display: flex; align-items: center; justify-content: center; min-height: 100vh; padding: 20px; }}
        .card {{ background: #1e293b; padding: 40px; border-radius: 16px; max-width: 500px; text-align: center; border: 1px solid #334155; }}
        h1 {{ font-size: 22px; margin-bottom: 12px; }}
        p {{ color: #94a3b8; line-height: 1.5; }}
    </style>
</head>
<body>
    <div class=""card"">
        <h1>⚙️ Scheduled Maintenance</h1>
        <p>{tenant.SchoolName} portal is undergoing routine performance maintenance and updates. Services will resume shortly.</p>
    </div>
</body>
</html>";
        }

        private static string GenerateTenantNotFoundHtml(string host)
        {
            var title = "School Portal Not Found";
            var heading = "School Portal Not Found";
            var description = $"The school portal address <strong>{host}</strong> is not recognized or has not been registered in the system yet.";

            return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>{title} - Exwhyzee Platform</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap"" rel=""stylesheet"">
    <style>
        * {{ box-sizing: border-box; margin: 0; padding: 0; }}
        body {{
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f172a 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
            color: #f8fafc;
        }}
        .card {{
            background: rgba(30, 41, 59, 0.7);
            backdrop-filter: blur(16px);
            border: 1px solid rgba(255, 255, 255, 0.1);
            border-radius: 24px;
            padding: 48px;
            max-width: 580px;
            width: 100%;
            text-align: center;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
        }}
        .icon-box {{
            width: 80px;
            height: 80px;
            border-radius: 20px;
            background: rgba(239, 68, 68, 0.15);
            border: 1px solid rgba(239, 68, 68, 0.3);
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 24px;
        }}
        .icon-box svg {{
            width: 40px;
            height: 40px;
            color: #f87171;
        }}
        h1 {{
            font-size: 26px;
            font-weight: 800;
            letter-spacing: -0.5px;
            margin-bottom: 12px;
            color: #ffffff;
        }}
        p {{
            font-size: 15px;
            line-height: 1.6;
            color: #94a3b8;
            margin-bottom: 24px;
        }}
        .badge {{
            display: inline-block;
            background: #0f172a;
            border: 1px solid #334155;
            padding: 8px 16px;
            border-radius: 12px;
            font-family: monospace;
            font-size: 14px;
            color: #38bdf8;
            margin-bottom: 28px;
            word-break: break-all;
        }}
        .actions {{
            display: flex;
            gap: 12px;
            justify-content: center;
            flex-wrap: wrap;
        }}
        .btn {{
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 12px 24px;
            border-radius: 12px;
            font-size: 14px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s ease;
        }}
        .btn-primary {{
            background: #2563eb;
            color: #ffffff;
            box-shadow: 0 10px 15px -3px rgba(37, 99, 235, 0.3);
        }}
        .btn-primary:hover {{
            background: #1d4ed8;
            transform: translateY(-1px);
        }}
        .btn-secondary {{
            background: rgba(255, 255, 255, 0.05);
            color: #cbd5e1;
            border: 1px solid rgba(255, 255, 255, 0.1);
        }}
        .btn-secondary:hover {{
            background: rgba(255, 255, 255, 0.1);
            color: #ffffff;
        }}
        .footer-note {{
            margin-top: 32px;
            padding-top: 20px;
            border-top: 1px solid rgba(255, 255, 255, 0.08);
            font-size: 12px;
            color: #64748b;
        }}
    </style>
</head>
<body>
    <div class=""card"">
        <div class=""icon-box"">
            <svg fill=""none"" stroke=""currentColor"" stroke-width=""2"" viewBox=""0 0 24 24"">
                <path stroke-linecap=""round"" stroke-linejoin=""round"" d=""M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"" />
            </svg>
        </div>
        <h1>{heading}</h1>
        <p>{description}</p>
        <div class=""badge"">{host}</div>
        <div class=""actions"">
            <a href=""mailto:support@exwhyzee.ng"" class=""btn btn-primary"">
                Contact Support
            </a>
            <a href=""javascript:location.reload()"" class=""btn btn-secondary"">
                Retry Connection
            </a>
        </div>
        <div class=""footer-note"">
            Exwhyzee Multi-Tenant Education Platform &bull; Security &amp; Isolation Shield
        </div>
    </div>
</body>
</html>";
        }
    }

    public static class TenantResolutionMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantResolutionMiddleware>();
        }
    }
}

