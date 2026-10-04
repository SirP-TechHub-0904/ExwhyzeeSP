using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Services;

namespace ExwhyzeeSP.Master.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
    public class AuditLogsController : Controller
    {
        private readonly MasterDbContext _context;
        private readonly IAuditLogService _auditService;

        public AuditLogsController(MasterDbContext context, IAuditLogService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: /AuditLogs
        [HttpGet("/AuditLogs")]
        public async Task<IActionResult> Index(
            string? searchString,
            string? actionFilter,
            string? severityFilter,
            string? areaFilter,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1)
        {
            const int pageSize = 50;
            var query = _context.AuditLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var s = searchString.Trim().ToLower();
                query = query.Where(l => l.UserName.ToLower().Contains(s) ||
                                         l.IpAddress.Contains(s) ||
                                         l.Action.ToLower().Contains(s) ||
                                         (l.Details != null && l.Details.ToLower().Contains(s)) ||
                                         (l.TenantSubDomain != null && l.TenantSubDomain.ToLower().Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(actionFilter))
            {
                query = query.Where(l => l.Action == actionFilter);
            }

            if (!string.IsNullOrWhiteSpace(severityFilter))
            {
                query = query.Where(l => l.Severity == severityFilter);
            }

            if (!string.IsNullOrWhiteSpace(areaFilter))
            {
                query = query.Where(l => l.Area == areaFilter);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.TimestampUtc >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(l => l.TimestampUtc <= endOfDay);
            }

            var totalCount = await query.CountAsync();
            var logs = await query
                .OrderByDescending(l => l.TimestampUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.ActionFilter = actionFilter;
            ViewBag.SeverityFilter = severityFilter;
            ViewBag.AreaFilter = areaFilter;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalCount = totalCount;

            // Distinct actions for dropdown filter
            ViewBag.AvailableActions = await _context.AuditLogs
                .Select(l => l.Action)
                .Distinct()
                .Take(25)
                .ToListAsync();

            return View(logs);
        }

        // GET: /AuditLogs/SsoSessions
        [HttpGet("/AuditLogs/SsoSessions")]
        public async Task<IActionResult> SsoSessions(int page = 1)
        {
            const int pageSize = 50;
            var query = _context.AuditLogs
                .AsNoTracking()
                .Where(l => l.Action.Contains("SSO") || l.Action.Contains("Delegated") || l.Action.Contains("Impersonat"));

            var totalCount = await query.CountAsync();
            var ssoLogs = await query
                .OrderByDescending(l => l.TimestampUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalCount = totalCount;

            return View(ssoLogs);
        }

        // GET: /AuditLogs/Details/5 (JSON)
        [HttpGet("/AuditLogs/Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var log = await _context.AuditLogs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            return Json(log);
        }

        // POST: /AuditLogs/Purge
        [HttpPost("/AuditLogs/Purge")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MasterRoles.SuperAdmin)]
        public async Task<IActionResult> Purge(int daysOlderThan, string? confirmText)
        {
            if (confirmText != "PURGE")
            {
                TempData["ErrorMessage"] = "Confirmation mismatch. Type 'PURGE' to confirm log clearing.";
                return RedirectToAction(nameof(Index));
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-Math.Abs(daysOlderThan));
            var toDelete = await _context.AuditLogs
                .Where(l => l.TimestampUtc < cutoffDate)
                .ToListAsync();

            var count = toDelete.Count;
            if (count > 0)
            {
                _context.AuditLogs.RemoveRange(toDelete);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    action: "AuditLogsPurged",
                    details: $"Cleared {count} audit log entries older than {cutoffDate:yyyy-MM-dd} ({daysOlderThan} days).",
                    area: "Security",
                    severity: "Warning");

                TempData["SuccessMessage"] = $"Successfully purged {count} audit logs older than {daysOlderThan} days ({cutoffDate:yyyy-MM-dd}).";
            }
            else
            {
                TempData["InfoMessage"] = $"No audit logs found older than {cutoffDate:yyyy-MM-dd}.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /AuditLogs/ExportCsv
        [HttpGet("/AuditLogs/ExportCsv")]
        public async Task<IActionResult> ExportCsv()
        {
            var logs = await _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(l => l.TimestampUtc)
                .Take(5000)
                .ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("Id,TimestampUtc,UserName,UserRole,Action,Area,Severity,IpAddress,Browser,DeviceType,OperatingSystem,TenantSubDomain,Details");

            foreach (var l in logs)
            {
                var cleanDetails = (l.Details ?? "").Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", "");
                builder.AppendLine($"\"{l.Id}\",\"{l.TimestampUtc:yyyy-MM-dd HH:mm:ss}\",\"{l.UserName}\",\"{l.UserRole}\",\"{l.Action}\",\"{l.Area}\",\"{l.Severity}\",\"{l.IpAddress}\",\"{l.Browser}\",\"{l.DeviceType}\",\"{l.OperatingSystem}\",\"{l.TenantSubDomain}\",\"{cleanDetails}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            return File(bytes, "text/csv", $"AuditLogs_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
    }
}
