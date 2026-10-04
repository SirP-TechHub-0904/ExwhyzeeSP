using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Services;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Master.Controllers
{
    public class CreateAdminUserViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = MasterRoles.Admin;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool RequirePasswordChangeOnLogin { get; set; } = true;
    }

    public class EditAdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = MasterRoles.Admin;

        public bool IsActive { get; set; } = true;

        public bool RequirePasswordChange { get; set; }

        public bool IsSuperAdminRoot { get; set; }
    }

    public class DetailsAdminUserViewModel
    {
        public MasterAdminUser User { get; set; } = null!;
        public IList<string> Roles { get; set; } = new List<string>();
        public bool IsSuperAdminRoot { get; set; }
        public int AuditLogCount { get; set; }
    }

    public class UserCreatedHandoffViewModel
    {
        public MasterAdminUser User { get; set; } = null!;
        public string Role { get; set; } = string.Empty;
        public string InitialPassword { get; set; } = string.Empty;
        public bool RequirePasswordChange { get; set; }
        public string LoginUrl { get; set; } = string.Empty;
    }

    public class ResetAdminPasswordViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "New Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool RequirePasswordChangeOnLogin { get; set; } = true;
    }

    [Area("Admin")]
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin},{MasterRoles.ReadOnly}")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<MasterAdminUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailLoggingService _emailService;
        private readonly IAuditLogService _auditService;
        private readonly MasterDbContext _context;

        public AdminUsersController(
            UserManager<MasterAdminUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailLoggingService emailService,
            IAuditLogService auditService,
            MasterDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _auditService = auditService;
            _context = context;
        }

        // GET: /Admin/AdminUsers
        public async Task<IActionResult> Index(string? search, string? role)
        {
            var isCurrentUserSuperAdmin = User.IsInRole(MasterRoles.SuperAdmin);
            var users = await _userManager.Users.OrderByDescending(u => u.CreatedDate).ToListAsync();
            var userListWithRoles = new List<(MasterAdminUser User, IList<string> Roles)>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var isTargetSuperAdmin = userRoles.Contains(MasterRoles.SuperAdmin) || user.RoleTitle == MasterRoles.SuperAdmin;

                // Root SuperAdmin is strictly excluded from standard User Management
                if (isTargetSuperAdmin)
                    continue;

                if (!string.IsNullOrEmpty(role) && !userRoles.Contains(role))
                    continue;

                if (!string.IsNullOrEmpty(search))
                {
                    var s = search.Trim().ToLowerInvariant();
                    var match = (user.FullName?.ToLowerInvariant().Contains(s) ?? false) ||
                                (user.Email?.ToLowerInvariant().Contains(s) ?? false) ||
                                (user.UserName?.ToLowerInvariant().Contains(s) ?? false);
                    if (!match) continue;
                }

                userListWithRoles.Add((user, userRoles));
            }

            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.IsCurrentUserSuperAdmin = isCurrentUserSuperAdmin;
            ViewBag.IsReadOnlyUser = User.IsInRole(MasterRoles.ReadOnly);
            ViewBag.RolesList = MasterRoles.AssignableRoles;

            return View(userListWithRoles);
        }

        // GET: /Admin/AdminUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var isTargetSuperAdmin = currentRoles.Contains(MasterRoles.SuperAdmin) || user.RoleTitle == MasterRoles.SuperAdmin;

            var auditLogCount = 0;
            if (!string.IsNullOrEmpty(user.Email) || !string.IsNullOrEmpty(user.UserName))
            {
                var identifier = user.Email ?? user.UserName;
                auditLogCount = await _context.AuditLogs.CountAsync(a => a.UserName == identifier || a.Details.Contains(identifier));
            }

            var vm = new DetailsAdminUserViewModel
            {
                User = user,
                Roles = currentRoles,
                IsSuperAdminRoot = isTargetSuperAdmin,
                AuditLogCount = auditLogCount
            };

            ViewBag.IsReadOnlyUser = User.IsInRole(MasterRoles.ReadOnly);
            return View(vm);
        }

        // GET: /Admin/AdminUsers/Create
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(MasterRoles.AssignableRoles);
            return View(new CreateAdminUserViewModel { Role = MasterRoles.Admin, RequirePasswordChangeOnLogin = true });
        }

        // POST: /Admin/AdminUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> Create(CreateAdminUserViewModel model)
        {
            if (model.Role == MasterRoles.SuperAdmin)
            {
                ModelState.AddModelError("Role", "The SuperAdmin role is strictly restricted to the root account and cannot be assigned to new users.");
            }

            if (ModelState.IsValid)
            {
                var existing = await _userManager.FindByEmailAsync(model.Email.Trim());
                if (existing != null)
                {
                    ModelState.AddModelError("Email", "An administrator with this email address already exists.");
                    ViewBag.Roles = new SelectList(MasterRoles.AssignableRoles, model.Role);
                    return View(model);
                }

                var user = new MasterAdminUser
                {
                    UserName = model.Email.Trim(),
                    Email = model.Email.Trim(),
                    FullName = model.FullName.Trim(),
                    PhoneNumber = model.PhoneNumber?.Trim(),
                    RoleTitle = model.Role,
                    IsActive = model.IsActive,
                    RequirePasswordChange = model.RequirePasswordChangeOnLogin,
                    EmailConfirmed = true,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    await _auditService.LogAsync(
                        action: "OperatorAccountCreated",
                        details: $"Created platform operator '{user.FullName}' ({user.Email}) with role '{model.Role}'. RequirePasswordChange: {model.RequirePasswordChangeOnLogin}.",
                        userName: User.Identity?.Name,
                        area: "Users",
                        severity: "Info",
                        httpContext: HttpContext);

                    TempData["SuccessMessage"] = $"Admin user '{user.FullName}' created successfully.";
                    return RedirectToAction(nameof(CreatedSuccess), new { id = user.Id, tempPass = model.Password });
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }

            ViewBag.Roles = new SelectList(MasterRoles.AssignableRoles, model.Role);
            return View(model);
        }

        // GET: /Admin/AdminUsers/CreatedSuccess
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> CreatedSuccess(string id, string? tempPass)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction(nameof(Index));

            var roles = await _userManager.GetRolesAsync(user);
            var hostUrl = $"{Request.Scheme}://{Request.Host}";

            var vm = new UserCreatedHandoffViewModel
            {
                User = user,
                Role = roles.FirstOrDefault() ?? user.RoleTitle,
                InitialPassword = tempPass ?? "[Hidden - Already Set]",
                RequirePasswordChange = user.RequirePasswordChange,
                LoginUrl = $"{hostUrl}/Account/Login"
            };

            return View(vm);
        }

        // POST: /Admin/AdminUsers/SendWelcomeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> SendWelcomeEmail(string userId, string? tempPass)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var hostUrl = $"{Request.Scheme}://{Request.Host}";
            var loginUrl = $"{hostUrl}/Account/Login";
            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault() ?? user.RoleTitle;

            var emailHtml = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; padding: 32px;'>
                <div style='text-align: center; margin-bottom: 24px;'>
                    <h2 style='color: #0284c7; margin: 0; font-size: 22px;'>ExwhyzeeSP Master Platform</h2>
                    <span style='color: #64748b; font-size: 13px; font-weight: bold;'>ADMINISTRATOR ACCESS CREDENTIALS</span>
                </div>
                <p>Hello <strong>{user.FullName}</strong>,</p>
                <p>You have been onboarded as a platform operator on the ExwhyzeeSP Master Control Plane with the role of <strong>{roleName}</strong>.</p>
                
                <div style='background: #f8fafc; border: 1px solid #cbd5e1; border-radius: 8px; padding: 20px; margin: 24px 0;'>
                    <div style='margin-bottom: 10px;'><strong>Portal URL:</strong> <a href='{loginUrl}' style='color: #0284c7;'>{loginUrl}</a></div>
                    <div style='margin-bottom: 10px;'><strong>Username / Email:</strong> <code style='background: #e2e8f0; padding: 2px 6px; border-radius: 4px;'>{user.Email}</code></div>
                    {(string.IsNullOrEmpty(tempPass) || tempPass.StartsWith("[Hidden") ? "" : $"<div style='margin-bottom: 10px;'><strong>Temporary Password:</strong> <code style='background: #e2e8f0; padding: 2px 6px; border-radius: 4px;'>{tempPass}</code></div>")}
                    <div><strong>Assigned Role:</strong> {roleName}</div>
                </div>

                {(user.RequirePasswordChange ? "<p style='color: #d97706; font-size: 13px;'><strong>Note:</strong> You will be required to choose a secure personal password upon your first login.</p>" : "")}

                <div style='text-align: center; margin: 28px 0;'>
                    <a href='{loginUrl}' style='background: #0284c7; color: #ffffff; text-decoration: none; padding: 12px 28px; border-radius: 8px; font-weight: bold; display: inline-block;'>Login to Admin Portal</a>
                </div>

                <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;' />
                <small style='color: #94a3b8;'>Exwhyzee SP Master Security Engine &bull; Automated Access Management</small>
            </div>";

            await _emailService.LogAndDumpEmailAsync(
                recipientEmail: user.Email!,
                subject: "Your ExwhyzeeSP Master Administrator Access Credentials",
                bodyHtml: emailHtml,
                recipientName: user.FullName,
                category: "AdminOnboarding",
                senderEmail: "admin@exwhyzee.ng");

            await _auditService.LogAsync(
                action: "WelcomeEmailDispatched",
                details: $"Welcome onboarding email sent to operator '{user.Email}'.",
                userName: User.Identity?.Name,
                area: "Users",
                severity: "Info",
                httpContext: HttpContext);

            TempData["SuccessMessage"] = $"Access credentials have been successfully emailed to {user.Email}.";
            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        // GET: /Admin/AdminUsers/Edit/5
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var isTargetSuperAdmin = currentRoles.Contains(MasterRoles.SuperAdmin) || user.RoleTitle == MasterRoles.SuperAdmin;

            if (isTargetSuperAdmin)
            {
                TempData["ErrorMessage"] = "Root SuperAdmin account cannot be managed from standard user management.";
                return RedirectToAction(nameof(Index));
            }

            var selectedRole = currentRoles.FirstOrDefault() ?? user.RoleTitle;

            var model = new EditAdminUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Role = selectedRole,
                IsActive = user.IsActive,
                RequirePasswordChange = user.RequirePasswordChange,
                IsSuperAdminRoot = false
            };

            ViewBag.Roles = new SelectList(MasterRoles.AssignableRoles, model.Role);
            return View(model);
        }

        // POST: /Admin/AdminUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> Edit(EditAdminUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var isTargetSuperAdmin = currentRoles.Contains(MasterRoles.SuperAdmin) || user.RoleTitle == MasterRoles.SuperAdmin;

            if (isTargetSuperAdmin)
            {
                TempData["ErrorMessage"] = "Root SuperAdmin settings cannot be edited from standard user management.";
                return RedirectToAction(nameof(Index));
            }

            if (model.Role == MasterRoles.SuperAdmin)
            {
                ModelState.AddModelError("Role", "The SuperAdmin role is strictly restricted to the root account.");
            }

            if (ModelState.IsValid)
            {
                user.FullName = model.FullName.Trim();
                user.PhoneNumber = model.PhoneNumber?.Trim();
                user.IsActive = model.IsActive;
                user.RequirePasswordChange = model.RequirePasswordChange;

                var effectiveRole = model.Role;
                user.RoleTitle = effectiveRole;

                if (user.Email != model.Email.Trim())
                {
                    user.Email = model.Email.Trim();
                    user.UserName = model.Email.Trim();
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (updateResult.Succeeded)
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (await _roleManager.RoleExistsAsync(effectiveRole))
                    {
                        await _userManager.AddToRoleAsync(user, effectiveRole);
                    }

                    await _auditService.LogAsync(
                        action: "OperatorUpdated",
                        details: $"Updated operator '{user.FullName}' ({user.Email}). Role: {effectiveRole}, Active: {user.IsActive}, RequirePasswordChange: {user.RequirePasswordChange}",
                        userName: User.Identity?.Name,
                        area: "Users",
                        severity: "Info",
                        httpContext: HttpContext);

                    TempData["SuccessMessage"] = $"Administrator '{user.FullName}' updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = user.Id });
                }

                foreach (var err in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }

            model.IsSuperAdminRoot = false;
            ViewBag.Roles = new SelectList(MasterRoles.AssignableRoles, model.Role);
            return View(model);
        }

        // POST: /Admin/AdminUsers/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> ToggleStatus(string id, string? reason)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains(MasterRoles.SuperAdmin) || user.RoleTitle == MasterRoles.SuperAdmin)
            {
                TempData["ErrorMessage"] = "The root SuperAdmin account cannot be deactivated.";
                return RedirectToAction(nameof(Index));
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user.Id == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot disable your own active administrator account.";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = !user.IsActive;
            if (!user.IsActive)
            {
                user.SuspendedAtUtc = DateTime.UtcNow;
                user.SuspensionReason = !string.IsNullOrWhiteSpace(reason) ? reason.Trim() : "Administrative suspension.";
            }
            else
            {
                user.SuspendedAtUtc = null;
                user.SuspensionReason = null;
            }

            await _userManager.UpdateAsync(user);

            await _auditService.LogAsync(
                action: user.IsActive ? "OperatorReactivated" : "OperatorSuspended",
                details: $"Operator '{user.FullName}' ({user.Email}) status changed to {(user.IsActive ? "ACTIVE" : "SUSPENDED")}. Reason: {user.SuspensionReason}",
                userName: User.Identity?.Name,
                area: "Users",
                severity: user.IsActive ? "Info" : "Warning",
                httpContext: HttpContext);

            TempData["SuccessMessage"] = $"Administrator account for '{user.FullName}' is now {(user.IsActive ? "Active" : "Suspended")}.";
            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        // POST: /Admin/AdminUsers/Disable2Fa/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> Disable2Fa(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.TwoFactorEnabled = false;
            user.TwoFactorChannel = "None";
            user.CurrentOtpHash = null;
            user.OtpExpiryUtc = null;
            await _userManager.UpdateAsync(user);

            await _auditService.LogAsync(
                action: "Operator2FaDisabledByAdmin",
                details: $"Two-Factor Authentication was reset/disabled for operator '{user.Email}'.",
                userName: User.Identity?.Name,
                area: "Security",
                severity: "Warning",
                httpContext: HttpContext);

            TempData["SuccessMessage"] = $"Two-Factor Authentication has been disabled for {user.FullName}.";
            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        // POST: /Admin/AdminUsers/ToggleRequirePasswordChange/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> ToggleRequirePasswordChange(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.RequirePasswordChange = !user.RequirePasswordChange;
            await _userManager.UpdateAsync(user);

            await _auditService.LogAsync(
                action: "RequirePasswordChangeToggled",
                details: $"Force password change flag set to {user.RequirePasswordChange} for '{user.Email}'.",
                userName: User.Identity?.Name,
                area: "Security",
                severity: "Info",
                httpContext: HttpContext);

            TempData["SuccessMessage"] = user.RequirePasswordChange
                ? $"{user.FullName} will be required to change their password on next login."
                : $"Password change requirement removed for {user.FullName}.";

            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        // GET: /Admin/AdminUsers/ResetPassword/5
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var isTargetSuperAdmin = currentRoles.Contains(MasterRoles.SuperAdmin) || user.RoleTitle == MasterRoles.SuperAdmin;

            if (isTargetSuperAdmin)
            {
                TempData["ErrorMessage"] = "Root SuperAdmin password cannot be reset from standard user management.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ResetAdminPasswordViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                RequirePasswordChangeOnLogin = true
            });
        }

        // POST: /Admin/AdminUsers/ResetPassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> ResetPassword(ResetAdminPasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var isTargetSuperAdmin = currentRoles.Contains(MasterRoles.SuperAdmin) || user.RoleTitle == MasterRoles.SuperAdmin;

            if (isTargetSuperAdmin)
            {
                TempData["ErrorMessage"] = "Root SuperAdmin password cannot be reset from standard user management.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (result.Succeeded)
                {
                    user.RequirePasswordChange = model.RequirePasswordChangeOnLogin;
                    await _userManager.UpdateAsync(user);

                    await _auditService.LogAsync(
                        action: "OperatorPasswordResetByAdmin",
                        details: $"Password reset for operator '{user.Email}'. RequirePasswordChange: {model.RequirePasswordChangeOnLogin}",
                        userName: User.Identity?.Name,
                        area: "Security",
                        severity: "Warning",
                        httpContext: HttpContext);

                    TempData["SuccessMessage"] = $"Password for '{user.FullName}' has been reset successfully.";
                    return RedirectToAction(nameof(Details), new { id = user.Id });
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }

            return View(model);
        }
    }
}
