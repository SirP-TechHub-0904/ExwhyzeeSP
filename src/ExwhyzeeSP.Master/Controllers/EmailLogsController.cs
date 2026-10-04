using System;
using System.Linq;
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
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin},{MasterRoles.ContentManager}")]
    public class EmailLogsController : Controller
    {
        private readonly MasterDbContext _context;
        private readonly IAuditLogService _auditService;

        public EmailLogsController(MasterDbContext context, IAuditLogService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: /EmailLogs & /Admin/EmailLogs
        [HttpGet("/EmailLogs")]
        [HttpGet("/Admin/EmailLogs")]
        public async Task<IActionResult> Index(
            string? searchString,
            string? categoryFilter,
            string? statusFilter,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1)
        {
            const int pageSize = 50;
            var query = _context.EmailLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var s = searchString.Trim().ToLower();
                query = query.Where(e => e.RecipientEmail.ToLower().Contains(s) ||
                                         e.Subject.ToLower().Contains(s) ||
                                         (e.RecipientName != null && e.RecipientName.ToLower().Contains(s)) ||
                                         (e.TriggeredBy != null && e.TriggeredBy.ToLower().Contains(s)) ||
                                         (e.TenantSubDomain != null && e.TenantSubDomain.ToLower().Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(categoryFilter))
            {
                query = query.Where(e => e.EmailCategory == categoryFilter);
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(e => e.Status == statusFilter);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(e => e.CreatedAtUtc >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(e => e.CreatedAtUtc <= endOfDay);
            }

            var totalCount = await query.CountAsync();
            var logs = await query
                .OrderByDescending(e => e.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalCount = totalCount;

            ViewBag.AvailableCategories = await _context.EmailLogs
                .Select(e => e.EmailCategory)
                .Distinct()
                .ToListAsync();

            return View(logs);
        }

        // GET: /EmailLogs/Preview/5
        [HttpGet("/EmailLogs/Preview/{id}")]
        public async Task<IActionResult> Preview(long id)
        {
            var emailLog = await _context.EmailLogs.FindAsync(id);
            if (emailLog == null)
            {
                return NotFound();
            }

            return Content(emailLog.BodyHtml ?? "<p>No content</p>", "text/html");
        }

        // GET: /EmailLogs/Details/5 (JSON)
        [HttpGet("/EmailLogs/Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var emailLog = await _context.EmailLogs.FindAsync(id);
            if (emailLog == null)
            {
                return NotFound();
            }

            return Json(emailLog);
        }

        // POST: /EmailLogs/Purge
        [HttpPost("/EmailLogs/Purge")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MasterRoles.SuperAdmin)]
        public async Task<IActionResult> Purge(int daysOlderThan, string? confirmText)
        {
            if (confirmText != "PURGE")
            {
                TempData["ErrorMessage"] = "Confirmation mismatch. Type 'PURGE' to confirm clearing email dumps.";
                return RedirectToAction(nameof(Index));
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-Math.Abs(daysOlderThan));
            var toDelete = await _context.EmailLogs
                .Where(e => e.CreatedAtUtc < cutoffDate)
                .ToListAsync();

            var count = toDelete.Count;
            if (count > 0)
            {
                _context.EmailLogs.RemoveRange(toDelete);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    action: "EmailLogsPurged",
                    details: $"Purged {count} email logs older than {cutoffDate:yyyy-MM-dd} ({daysOlderThan} days).",
                    area: "Security",
                    severity: "Warning");

                TempData["SuccessMessage"] = $"Successfully purged {count} email logs older than {daysOlderThan} days ({cutoffDate:yyyy-MM-dd}).";
            }
            else
            {
                TempData["InfoMessage"] = $"No email logs found older than {cutoffDate:yyyy-MM-dd}.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
