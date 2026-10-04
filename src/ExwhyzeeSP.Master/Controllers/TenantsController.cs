using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Master.Models;
using ExwhyzeeSP.Services;
using ExwhyzeeSP.Services.Provisioning;

namespace ExwhyzeeSP.Master.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin},{MasterRoles.ReadOnly}")]
    public class TenantsController : Controller
    {
        private readonly MasterDbContext _context;
        private readonly IDatabaseProvisioningService _provisioningService;
        private readonly ITenantMigrationService _migrationService;
        private readonly IDelegatedSsoService _ssoService;
        private readonly IAuditLogService _auditService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly UserManager<MasterAdminUser> _userManager;
        private readonly SignInManager<MasterAdminUser> _signInManager;
        private readonly ITenantBackupService _backupService;

        public TenantsController(
            MasterDbContext context,
            IDatabaseProvisioningService provisioningService,
            ITenantMigrationService migrationService,
            IDelegatedSsoService ssoService,
            IAuditLogService auditService,
            IConfiguration configuration,
            IMemoryCache cache,
            UserManager<MasterAdminUser> userManager,
            SignInManager<MasterAdminUser> signInManager,
            ITenantBackupService backupService)
        {
            _context = context;
            _provisioningService = provisioningService;
            _migrationService = migrationService;
            _ssoService = ssoService;
            _auditService = auditService;
            _configuration = configuration;
            _cache = cache;
            _userManager = userManager;
            _signInManager = signInManager;
            _backupService = backupService;
        }

        // GET: /Tenants
        public async Task<IActionResult> Index(string? searchString, string? statusFilter, int page = 1, int pageSize = 12)
        {
            if (page < 1) page = 1;
            if (pageSize < 5) pageSize = 12;
            if (pageSize > 100) pageSize = 100;

            var query = _context.TenantSchools.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(s => s.SchoolName.Contains(searchString) ||
                                         (s.SubDomain != null && s.SubDomain.Contains(searchString)) ||
                                         (s.HostDomain != null && s.HostDomain.Contains(searchString)));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                if (statusFilter == "active") query = query.Where(s => s.IsActive);
                else if (statusFilter == "inactive") query = query.Where(s => !s.IsActive);
            }

            var totalItems = await query.CountAsync();
            var totalSchools = await _context.TenantSchools.CountAsync();
            var activeSchools = await _context.TenantSchools.CountAsync(s => s.IsActive);

            var schools = await query
                .OrderByDescending(s => s.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Check migration status ONLY for the current page items with 3-minute memory cache
            var statuses = new Dictionary<int, TenantMigrationStatus>();
            var tasks = schools.Select(async school =>
            {
                var cacheKey = $"tenant_status_{school.Id}";
                if (!_cache.TryGetValue(cacheKey, out TenantMigrationStatus? cachedStatus) || cachedStatus == null)
                {
                    cachedStatus = await _migrationService.CheckStatusAsync(school);
                    _cache.Set(cacheKey, cachedStatus, TimeSpan.FromMinutes(3));
                }
                return (school.Id, cachedStatus);
            });

            var results = await Task.WhenAll(tasks);
            foreach (var (schoolId, status) in results)
            {
                statuses[schoolId] = status;
            }

            ViewBag.SearchString = searchString;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalSchools = totalSchools;
            ViewBag.ActiveSchools = activeSchools;
            ViewBag.PendingUpdatesCount = statuses.Values.Count(s => s.HasPendingMigrations);
            ViewBag.ConnectionErrorsCount = statuses.Values.Count(s => !s.CanConnect);
            ViewBag.MigrationStatuses = statuses;

            return View(schools);
        }


        // GET: /Tenants/Create
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public IActionResult Create()
        {
            var defaultHost = Environment.GetEnvironmentVariable("SQL_SERVER_HOST")
                ?? _configuration["DatabaseServer:Host"]
                ?? "JSS\\SQLEXPRESS";

            var defaultPortStr = Environment.GetEnvironmentVariable("SQL_SERVER_PORT")
                ?? _configuration["DatabaseServer:Port"];
            int.TryParse(defaultPortStr, out int defaultPort);

            var defaultAdminUser = Environment.GetEnvironmentVariable("SQL_ADMIN_USER")
                ?? _configuration["DatabaseServer:AdminUser"]
                ?? "sa";

            var defaultAdminPassword = Environment.GetEnvironmentVariable("SQL_ADMIN_PASSWORD")
                ?? _configuration["DatabaseServer:AdminPassword"];

            var useIntegrated = bool.TryParse(Environment.GetEnvironmentVariable("SQL_USE_INTEGRATED_SECURITY"), out bool integrated) && integrated;

            var model = new CreateTenantViewModel
            {
                ServerHost = defaultHost,
                ServerPort = defaultPort > 0 ? defaultPort : null,
                AdminUser = defaultAdminUser,
                AdminPassword = defaultAdminPassword,
                UseIntegratedSecurity = useIntegrated,
                ProvisionMode = DatabaseProvisionMode.AutoCreateDatabase,
                RunMigrationsImmediately = true,
                IsActive = true
            };

            return View(model);
        }

        // POST: /Tenants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> Create(CreateTenantViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SchoolName))
            {
                ModelState.AddModelError(nameof(model.SchoolName), "School name is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SubDomain))
            {
                ModelState.AddModelError(nameof(model.SubDomain), "Subdomain prefix is required.");
            }
            else
            {
                model.SubDomain = model.SubDomain.Trim().ToLowerInvariant();
                var exists = await _context.TenantSchools.AnyAsync(t => t.SubDomain == model.SubDomain);
                if (exists)
                {
                    ModelState.AddModelError(nameof(model.SubDomain), $"Subdomain '{model.SubDomain}' is already assigned to another school.");
                }
            }

            string finalConnectionString = string.Empty;

            if (model.ProvisionMode == DatabaseProvisionMode.ExistingConnectionString)
            {
                if (string.IsNullOrWhiteSpace(model.ConnectionString))
                {
                    ModelState.AddModelError(nameof(model.ConnectionString), "Please provide a valid SQL connection string.");
                }
                else
                {
                    finalConnectionString = model.ConnectionString.Trim();
                }
            }
            else // AutoCreateDatabase
            {
                if (string.IsNullOrWhiteSpace(model.ServerHost))
                {
                    ModelState.AddModelError(nameof(model.ServerHost), "Server IP or Hostname is required for auto-provisioning.");
                }

                if (!model.UseIntegratedSecurity && string.IsNullOrWhiteSpace(model.AdminUser))
                {
                    ModelState.AddModelError(nameof(model.AdminUser), "SQL Admin Username (e.g. sa) is required for provisioning.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Execute auto-provisioning if selected
            if (model.ProvisionMode == DatabaseProvisionMode.AutoCreateDatabase)
            {
                var cleanSubdomain = Regex.Replace(model.SubDomain, @"[^a-zA-Z0-9_]", "_");
                var dbName = !string.IsNullOrWhiteSpace(model.CustomDatabaseName)
                    ? model.CustomDatabaseName.Trim()
                    : $"db_school_{cleanSubdomain}";

                var username = !string.IsNullOrWhiteSpace(model.CustomUsername)
                    ? model.CustomUsername.Trim()
                    : $"usr_{cleanSubdomain}";

                var password = !string.IsNullOrWhiteSpace(model.CustomPassword)
                    ? model.CustomPassword.Trim()
                    : _provisioningService.GenerateSecurePassword(16);

                var serverConfig = new DatabaseServerConfig
                {
                    ServerHost = model.ServerHost.Trim(),
                    Port = model.ServerPort,
                    AdminUser = model.AdminUser?.Trim(),
                    AdminPassword = model.AdminPassword,
                    UseIntegratedSecurity = model.UseIntegratedSecurity
                };

                var provResult = await _provisioningService.ProvisionDatabaseAndUserAsync(serverConfig, dbName, username, password);
                if (!provResult.Success)
                {
                    ModelState.AddModelError(string.Empty, provResult.Message);
                    return View(model);
                }

                finalConnectionString = provResult.ConnectionString;
            }

            // Save TenantSchool record into Master Database
            var tenant = new TenantSchool
            {
                SchoolName = model.SchoolName.Trim(),
                SubDomain = model.SubDomain.Trim().ToLowerInvariant(),
                HostDomain = string.IsNullOrWhiteSpace(model.HostDomain) ? null : model.HostDomain.Trim().ToLowerInvariant(),
                ConnectionString = finalConnectionString,
                IsActive = model.IsActive,
                DateCreated = DateTime.UtcNow
            };

            _context.TenantSchools.Add(tenant);
            await _context.SaveChangesAsync();

            // Run EF Migrations immediately if selected
            string migrationStatus = string.Empty;
            if (model.RunMigrationsImmediately)
            {
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlServer(tenant.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("ExwhyzeeSP.Data");
                        sqlOptions.CommandTimeout(180);
                    });

                    using var tenantContext = new ApplicationDbContext(optionsBuilder.Options);
                    var pendingMigrations = (await tenantContext.Database.GetPendingMigrationsAsync()).ToList();
                    if (pendingMigrations.Any())
                    {
                        await tenantContext.Database.MigrateAsync();
                        migrationStatus = $" & Schema migrated ({pendingMigrations.Count} tables/migrations applied)";
                    }
                }
                catch (Exception ex)
                {
                    migrationStatus = $" (Note: Database created, but schema migration failed: {ex.Message}. You can click 'Run Migration' in the directory list to retry).";
                }
            }

            TempData["SuccessMessage"] = $"School '{tenant.SchoolName}' provisioned successfully! URL: {tenant.SubDomain}.exwhyzee.ng{migrationStatus}";
            return RedirectToAction(nameof(Index));
        }


        // GET: /Tenants/Edit/5
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tenant = await _context.TenantSchools.FindAsync(id);
            if (tenant == null) return NotFound();

            return View(tenant);
        }

        // POST: /Tenants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> Edit(int id, TenantSchool tenant)
        {
            if (id != tenant.Id) return NotFound();

            if (string.IsNullOrWhiteSpace(tenant.SchoolName))
            {
                ModelState.AddModelError("SchoolName", "School name is required.");
            }

            if (string.IsNullOrWhiteSpace(tenant.ConnectionString))
            {
                ModelState.AddModelError("ConnectionString", "Connection string is required.");
            }

            if (!string.IsNullOrWhiteSpace(tenant.SubDomain))
            {
                tenant.SubDomain = tenant.SubDomain.Trim().ToLowerInvariant();
                var exists = await _context.TenantSchools.AnyAsync(t => t.SubDomain == tenant.SubDomain && t.Id != id);
                if (exists)
                {
                    ModelState.AddModelError("SubDomain", "This subdomain is already assigned to another school.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.TenantSchools.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.SchoolName = tenant.SchoolName;
                    existing.SubDomain = tenant.SubDomain;
                    existing.HostDomain = tenant.HostDomain;
                    existing.ConnectionString = tenant.ConnectionString;
                    existing.IsActive = tenant.IsActive;
                    existing.LastModified = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"School '{tenant.SchoolName}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.TenantSchools.AnyAsync(e => e.Id == tenant.Id))
                        return NotFound();
                    throw;
                }
            }
            return View(tenant);
        }

        // GET: /Tenants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tenant = await _context.TenantSchools.FirstOrDefaultAsync(m => m.Id == id);
            if (tenant == null) return NotFound();

            var migrationStatus = await _migrationService.CheckStatusAsync(tenant);
            ViewBag.MigrationStatus = migrationStatus;
            ViewBag.BackupHistories = await _backupService.GetBackupHistoryAsync(tenant.Id);

            return View(tenant);
        }


        // POST: /Tenants/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var tenant = await _context.TenantSchools.FindAsync(id);
            if (tenant != null)
            {
                tenant.IsActive = !tenant.IsActive;
                tenant.LastModified = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Status for '{tenant.SchoolName}' updated to {(tenant.IsActive ? "Active" : "Disabled")}.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Tenants/MigrateTenant/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> MigrateTenant(int id)
        {
            var tenant = await _context.TenantSchools.FindAsync(id);
            if (tenant == null) return NotFound();

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(tenant.ConnectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("ExwhyzeeSP.Data");
                    sqlOptions.CommandTimeout(180);
                });

                using var tenantContext = new ApplicationDbContext(optionsBuilder.Options);
                var pendingMigrations = (await tenantContext.Database.GetPendingMigrationsAsync()).ToList();

                if (pendingMigrations.Any())
                {
                    await tenantContext.Database.MigrateAsync();
                    TempData["SuccessMessage"] = $"Migration succeeded for '{tenant.SchoolName}'! Applied {pendingMigrations.Count} migration(s): {string.Join(", ", pendingMigrations)}.";
                }
                else
                {
                    TempData["InfoMessage"] = $"Database for '{tenant.SchoolName}' is already up to date. No pending migrations.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Migration failed for '{tenant.SchoolName}': {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Tenants/MigrateAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> MigrateAll()
        {
            var activeTenants = await _context.TenantSchools.Where(t => t.IsActive).ToListAsync();
            int updatedCount = 0;
            int upToDateCount = 0;
            int failedCount = 0;
            var errorList = new List<string>();

            foreach (var tenant in activeTenants)
            {
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlServer(tenant.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("ExwhyzeeSP.Data");
                        sqlOptions.CommandTimeout(180);
                    });

                    using var tenantContext = new ApplicationDbContext(optionsBuilder.Options);
                    var pendingMigrations = (await tenantContext.Database.GetPendingMigrationsAsync()).ToList();

                    if (pendingMigrations.Any())
                    {
                        await tenantContext.Database.MigrateAsync();
                        updatedCount++;
                    }
                    else
                    {
                        upToDateCount++;
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    errorList.Add($"{tenant.SchoolName} ({ex.Message})");
                }
            }

            var summary = $"Batch Migration Complete: {updatedCount} updated, {upToDateCount} already up-to-date, {failedCount} failed.";
            if (failedCount > 0)
            {
                TempData["ErrorMessage"] = $"{summary} Failures: {string.Join("; ", errorList)}";
            }
            else
            {
                TempData["SuccessMessage"] = summary;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Tenants/TestConnection (AJAX Safe Ping)
        [HttpPost]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> TestConnection([FromBody] TestConnectionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.ConnectionString))
            {
                return Json(new { success = false, message = "Connection string cannot be empty." });
            }

            try
            {
                using var conn = new SqlConnection(request.ConnectionString);
                await conn.OpenAsync();
                return Json(new { success = true, message = "Connection successful! Database server responded." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Connection failed: {ex.Message}" });
            }
        }

        // GET: /Tenants/AccessControl/5
        [HttpGet]
        public async Task<IActionResult> AccessControl(int id)
        {
            var school = await _context.TenantSchools.FindAsync(id);
            if (school == null)
            {
                TempData["ErrorMessage"] = "School tenant not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(school);
        }

        // POST: /Tenants/AccessControl/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> AccessControl(int id, TenantSchool model)
        {
            var school = await _context.TenantSchools.FindAsync(id);
            if (school == null)
            {
                TempData["ErrorMessage"] = "School tenant not found.";
                return RedirectToAction(nameof(Index));
            }

            school.IsActive = model.IsActive;
            school.SuspensionMode = model.SuspensionMode;
            school.SuspensionReason = model.SuspensionReason;
            school.SuspensionNotice = model.SuspensionNotice;
            school.GracePeriodDeadline = model.GracePeriodDeadline;

            // Feature gates
            school.AllowResultChecking = model.AllowResultChecking;
            school.AllowCbtExams = model.AllowCbtExams;
            school.AllowOnlinePayment = model.AllowOnlinePayment;
            school.AllowStaffPortal = model.AllowStaffPortal;
            school.AllowStudentPortal = model.AllowStudentPortal;
            school.AllowAdmissionPortal = model.AllowAdmissionPortal;
            school.AllowSmsNotifications = model.AllowSmsNotifications;
            school.AllowElearning = model.AllowElearning;
            school.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Access control and feature gatekeeper policies updated for {school.SchoolName}.";
            return RedirectToAction(nameof(Details), new { id = school.Id });
        }

        // POST: /Tenants/QuickLock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> QuickLock(int id, TenantSuspensionMode mode, string? reason = null)
        {
            var school = await _context.TenantSchools.FindAsync(id);
            if (school == null)
            {
                TempData["ErrorMessage"] = "School tenant not found.";
                return RedirectToAction(nameof(Index));
            }

            school.SuspensionMode = mode;
            if (mode == TenantSuspensionMode.Active)
            {
                school.IsActive = true;
                school.SuspensionReason = null;
                school.SuspensionNotice = null;
            }
            else
            {
                school.SuspensionReason = reason ?? $"Quick Lock applied from Master Administration ({mode}).";
                school.SuspensionNotice = $"This school's { (mode == TenantSuspensionMode.PortalDashboardSuspensionOnly ? "portal dashboards are" : "platform access is") } temporarily restricted pending subscription verification. Contact administrative billing.";
            }

            school.LastModified = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Status updated to {mode} for {school.SchoolName}.";
            return RedirectToAction(nameof(Details), new { id = school.Id });
        }

        // POST: /Tenants/LaunchImpersonation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> LaunchImpersonation(int id, string role = "SuperAdmin", string? reason = null)
        {
            var school = await _context.TenantSchools.FindAsync(id);
            if (school == null)
            {
                TempData["ErrorMessage"] = "School tenant not found.";
                return RedirectToAction(nameof(Index));
            }

            var masterEmail = User.Identity?.Name ?? "master.admin@exwhyzee.ng";
            var supportReason = string.IsNullOrWhiteSpace(reason)
                ? "Platform Diagnostics & Administration Inspection"
                : reason.Trim();

            // Generate cryptographically signed single-use delegated token valid for 60 seconds
            var token = _ssoService.GenerateDelegatedToken(masterEmail, school.SubDomain ?? string.Empty, role, supportReason, 60);

            await _auditService.LogAsync(
                action: "DelegatedSsoLaunch",
                details: $"Master user requested support access as '{role}' into '{school.SchoolName}' ({school.SubDomain}). Reason: {supportReason}",
                userName: masterEmail,
                area: "Security",
                severity: "Warning",
                tenantSubDomain: school.SubDomain,
                tenantSchoolId: school.Id);

            // Determine target host URL
            var targetHost = !string.IsNullOrWhiteSpace(school.HostDomain)
                ? $"https://{school.HostDomain}"
                : (!string.IsNullOrWhiteSpace(school.SubDomain)
                    ? $"https://{school.SubDomain}.exwhyzee.ng"
                    : "/");

            // For local development or relative proxy
            var redirectUrl = $"/Auth/DelegatedLogin?token={Uri.EscapeDataString(token)}";
            if (!string.IsNullOrWhiteSpace(school.HostDomain) || !string.IsNullOrWhiteSpace(school.SubDomain))
            {
                redirectUrl = $"{targetHost}/Auth/DelegatedLogin?token={Uri.EscapeDataString(token)}";
            }

            // Return direct redirect or JSON if called via modal
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, redirectUrl = redirectUrl, token = token });
            }

            return Redirect(redirectUrl);
        }

        // GET: /Tenants/Delete/5
        [HttpGet]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            var school = await _context.TenantSchools
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (school == null)
            {
                TempData["ErrorMessage"] = "School institution not found or already decommissioned.";
                return RedirectToAction(nameof(Index));
            }

            var transactions = await _context.SchoolTransactions
                .Where(t => t.TenantSchoolId == id)
                .ToListAsync();

            var viewModel = new DeleteTenantViewModel
            {
                Id = school.Id,
                SchoolName = school.SchoolName,
                SubDomain = school.SubDomain,
                HostDomain = school.HostDomain,
                DateCreated = school.DateCreated,
                IsActive = school.IsActive,
                TransactionCount = transactions.Count,
                TransactionVolume = transactions.Sum(t => t.Amount)
            };

            return View(viewModel);
        }

        // POST: /Tenants/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> DeleteConfirmed(DeleteTenantViewModel model)
        {
            var school = await _context.TenantSchools
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == model.Id && !m.IsDeleted);

            if (school == null)
            {
                TempData["ErrorMessage"] = "School institution not found or already decommissioned.";
                return RedirectToAction(nameof(Index));
            }

            // 1. Verify Confirmation Phrase (must be "DELETE" or "DELETE <SchoolName>")
            var expectedPhrase1 = "DELETE";
            var expectedPhrase2 = $"DELETE {school.SchoolName}".Trim();
            var enteredPhrase = model.ConfirmationText?.Trim() ?? string.Empty;

            if (!string.Equals(enteredPhrase, expectedPhrase1, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(enteredPhrase, expectedPhrase2, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(enteredPhrase, school.SchoolName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.ConfirmationText), $"Confirmation phrase does not match. Please enter 'DELETE' or 'DELETE {school.SchoolName}'.");
            }

            // 2. Verify Admin Password
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null)
            {
                return Challenge();
            }

            if (string.IsNullOrWhiteSpace(model.AdminPassword))
            {
                ModelState.AddModelError(nameof(model.AdminPassword), "Administrator password is required to authorize decommissioning.");
            }
            else
            {
                var passwordValid = await _userManager.CheckPasswordAsync(adminUser, model.AdminPassword);
                if (!passwordValid)
                {
                    ModelState.AddModelError(nameof(model.AdminPassword), "Incorrect administrator password. Deletion authorization failed.");
                }
            }

            if (!ModelState.IsValid)
            {
                var transactions = await _context.SchoolTransactions
                    .Where(t => t.TenantSchoolId == school.Id)
                    .ToListAsync();

                model.SchoolName = school.SchoolName;
                model.SubDomain = school.SubDomain;
                model.HostDomain = school.HostDomain;
                model.DateCreated = school.DateCreated;
                model.IsActive = school.IsActive;
                model.TransactionCount = transactions.Count;
                model.TransactionVolume = transactions.Sum(t => t.Amount);

                return View("Delete", model);
            }

            // 3. Capture Snapshot for Compliance Audit Log
            var originalName = school.SchoolName;
            var originalSubDomain = school.SubDomain;
            var originalHostDomain = school.HostDomain;
            var originalConnection = school.ConnectionString;

            await _auditService.LogAsync(
                action: "TenantDecommissioned",
                details: $"School institution '{originalName}' (ID: #{school.Id}, Subdomain: '{originalSubDomain}', CustomDomain: '{originalHostDomain}') was permanently decommissioned by {adminUser.Email}. Reason: {model.Reason}",
                userName: adminUser.Email ?? adminUser.UserName ?? "Admin",
                area: "Tenants",
                severity: "Critical",
                tenantSubDomain: originalSubDomain,
                tenantSchoolId: school.Id);

            // 4. FinTech Soft Delete & Anonymization
            // Ensures financial ledger relationships and database keys are preserved while wiping PII and freeing up domain routing
            school.IsDeleted = true;
            school.IsActive = false;
            school.SuspensionMode = TenantSuspensionMode.FullSuspension;
            school.SuspensionReason = $"Decommissioned on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC by {adminUser.Email}. Reason: {model.Reason}";
            school.SuspensionNotice = "This school portal has been permanently decommissioned and archived.";
            school.SchoolName = $"[DELETED] {originalName} (Archived #{school.Id})";
            school.SubDomain = $"deleted-{school.Id}-{Guid.NewGuid():N}";
            school.HostDomain = null;
            school.ConnectionString = "WIPED_AND_DECOMMISSIONED";
            school.DeletedAt = DateTime.UtcNow;
            school.DeletedBy = adminUser.Email;
            school.DeletionReason = model.Reason;
            school.LastModified = DateTime.UtcNow;

            // Clear cache
            _cache.Remove($"tenant_status_{school.Id}");

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Institution '{originalName}' has been successfully decommissioned and archived. All domain and portal routing has been permanently revoked.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Tenants/DownloadBackup/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> DownloadBackup(int id)
        {
            var school = await _context.TenantSchools.FindAsync(id);
            if (school == null)
            {
                TempData["ErrorMessage"] = "School tenant not found.";
                return RedirectToAction(nameof(Index));
            }

            var backupResult = await _backupService.GenerateDatabaseBackupAsync(school, User.Identity?.Name, recordHistory: true);
            if (!backupResult.Success || backupResult.FileBytes == null)
            {
                TempData["ErrorMessage"] = backupResult.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            // Log download audit event
            await _auditService.LogAsync(
                action: "DatabaseBackupDownloaded",
                details: $"Database backup archive '{backupResult.FileName}' ({backupResult.FileSizeFormatted}) downloaded by {User.Identity?.Name}.",
                userName: User.Identity?.Name ?? "Admin",
                area: "Tenants",
                severity: "Information",
                tenantSubDomain: school.SubDomain,
                tenantSchoolId: school.Id);

            return File(backupResult.FileBytes, "application/octet-stream", backupResult.FileName);
        }

        // POST: /Tenants/UploadBackupToCloudinary/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> UploadBackupToCloudinary(int id)
        {
            var school = await _context.TenantSchools.FindAsync(id);
            if (school == null)
            {
                TempData["ErrorMessage"] = "School tenant not found.";
                return RedirectToAction(nameof(Index));
            }

            var backupResult = await _backupService.GenerateDatabaseBackupAsync(school, User.Identity?.Name, recordHistory: false);
            if (!backupResult.Success || backupResult.FileBytes == null || string.IsNullOrWhiteSpace(backupResult.FileName))
            {
                TempData["ErrorMessage"] = backupResult.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            var uploadResult = await _backupService.UploadToCloudinaryAsync(school, backupResult.FileBytes, backupResult.FileName, User.Identity?.Name);
            if (uploadResult.Success)
            {
                await _auditService.LogAsync(
                    action: "DatabaseBackupCloudVaulted",
                    details: $"Database backup securely vaulted to Cloudinary for '{school.SchoolName}' ({uploadResult.FileSizeFormatted}) by {User.Identity?.Name}. URL: {uploadResult.CloudUrl}",
                    userName: User.Identity?.Name ?? "Admin",
                    area: "Tenants",
                    severity: "Information",
                    tenantSubDomain: school.SubDomain,
                    tenantSchoolId: school.Id);

                TempData["SuccessMessage"] = uploadResult.Message;
            }
            else
            {
                TempData["ErrorMessage"] = uploadResult.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }

    public class TestConnectionRequest
    {
        public string ConnectionString { get; set; } = string.Empty;
    }
}

