using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Services;
using ExwhyzeeSP.Services.Provisioning;

namespace ExwhyzeeSP.Master.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin},{MasterRoles.ReadOnly}")]
    public class BackupsController : Controller
    {
        private readonly MasterDbContext _context;
        private readonly ITenantBackupService _backupService;
        private readonly IAuditLogService _auditService;

        public BackupsController(
            MasterDbContext context,
            ITenantBackupService backupService,
            IAuditLogService auditService)
        {
            _context = context;
            _backupService = backupService;
            _auditService = auditService;
        }

        // GET: /Admin/Backups
        public async Task<IActionResult> Index(
            string? searchString = null,
            int? schoolId = null,
            string? typeFilter = null,
            string? statusFilter = null,
            int page = 1,
            int pageSize = 20)
        {
            var query = _context.TenantBackupHistories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim();
                query = query.Where(b => b.SchoolName.Contains(term) || (b.Subdomain != null && b.Subdomain.Contains(term)) || (b.FileName != null && b.FileName.Contains(term)));
            }

            if (schoolId.HasValue && schoolId.Value > 0)
            {
                query = query.Where(b => b.TenantSchoolId == schoolId.Value);
            }

            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                query = query.Where(b => b.BackupType == typeFilter);
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(b => b.Status == statusFilter);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var items = await query
                .OrderByDescending(b => b.BackupDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Aggregate metrics
            var totalBackups = await _context.TenantBackupHistories.CountAsync();
            var cloudVaultBackups = await _context.TenantBackupHistories.CountAsync(b => b.BackupType == "Cloudinary Vault" && b.Status == "Success");
            var directDownloads = await _context.TenantBackupHistories.CountAsync(b => b.BackupType == "Local Download");
            var failedBackups = await _context.TenantBackupHistories.CountAsync(b => b.Status == "Failed");

            // Available active schools for quick trigger
            var schools = await _context.TenantSchools
                .Where(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.SchoolName)
                .Select(s => new { s.Id, s.SchoolName, s.SubDomain })
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.SchoolId = schoolId;
            ViewBag.TypeFilter = typeFilter;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalBackups = totalBackups;
            ViewBag.CloudVaultBackups = cloudVaultBackups;
            ViewBag.DirectDownloads = directDownloads;
            ViewBag.FailedBackups = failedBackups;
            ViewBag.Schools = schools;

            return View(items);
        }

        // POST: /Admin/Backups/TriggerVault
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> TriggerVault(int schoolId)
        {
            var school = await _context.TenantSchools.FindAsync(schoolId);
            if (school == null)
            {
                TempData["ErrorMessage"] = "School tenant was not found.";
                return RedirectToAction(nameof(Index));
            }

            var backupResult = await _backupService.GenerateDatabaseBackupAsync(school, User.Identity?.Name, recordHistory: false);
            if (!backupResult.Success || backupResult.FileBytes == null || string.IsNullOrWhiteSpace(backupResult.FileName))
            {
                TempData["ErrorMessage"] = $"Backup failed: {backupResult.Message}";
                return RedirectToAction(nameof(Index));
            }

            var uploadResult = await _backupService.UploadToCloudinaryAsync(school, backupResult.FileBytes, backupResult.FileName, User.Identity?.Name);
            if (uploadResult.Success)
            {
                await _auditService.LogAsync(
                    action: "DatabaseBackupCloudVaulted",
                    details: $"Database backup securely vaulted to Cloudinary for '{school.SchoolName}' ({uploadResult.FileSizeFormatted}) by {User.Identity?.Name}. URL: {uploadResult.CloudUrl}",
                    userName: User.Identity?.Name ?? "Admin",
                    area: "Backups",
                    severity: "Information",
                    tenantSubDomain: school.SubDomain,
                    tenantSchoolId: school.Id);

                TempData["SuccessMessage"] = uploadResult.Message;
            }
            else
            {
                TempData["ErrorMessage"] = uploadResult.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Backups/TriggerDownload/{schoolId}
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> TriggerDownload(int schoolId)
        {
            var school = await _context.TenantSchools.FindAsync(schoolId);
            if (school == null)
            {
                TempData["ErrorMessage"] = "School tenant not found.";
                return RedirectToAction(nameof(Index));
            }

            var backupResult = await _backupService.GenerateDatabaseBackupAsync(school, User.Identity?.Name, recordHistory: true);
            if (!backupResult.Success || backupResult.FileBytes == null)
            {
                TempData["ErrorMessage"] = backupResult.Message;
                return RedirectToAction(nameof(Index));
            }

            await _auditService.LogAsync(
                action: "DatabaseBackupDownloaded",
                details: $"Database backup archive '{backupResult.FileName}' ({backupResult.FileSizeFormatted}) generated and downloaded by {User.Identity?.Name}.",
                userName: User.Identity?.Name ?? "Admin",
                area: "Backups",
                severity: "Information",
                tenantSubDomain: school.SubDomain,
                tenantSchoolId: school.Id);

            return File(backupResult.FileBytes, "application/octet-stream", backupResult.FileName);
        }

        // POST: /Admin/Backups/DeleteHistory/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MasterRoles.SuperAdmin)]
        public async Task<IActionResult> DeleteHistory(long id)
        {
            var record = await _context.TenantBackupHistories.FindAsync(id);
            if (record != null)
            {
                _context.TenantBackupHistories.Remove(record);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Backup history record deleted from audit log.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
