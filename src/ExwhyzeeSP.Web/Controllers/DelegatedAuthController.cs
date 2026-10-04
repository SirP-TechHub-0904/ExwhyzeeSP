using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Data.Identity;
using ExwhyzeeSP.Data.MultiTenancy;
using ExwhyzeeSP.Services;

namespace ExwhyzeeSP.Web.Controllers
{
    [AllowAnonymous]
    public class DelegatedAuthController : Controller
    {
        private readonly IDelegatedSsoService _ssoService;
        private readonly ITenantService _tenantService;
        private readonly IAuditLogService _auditService;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationRoleManager _roleManager;

        public DelegatedAuthController(
            IDelegatedSsoService ssoService,
            ITenantService tenantService,
            IAuditLogService auditService,
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            ApplicationRoleManager roleManager)
        {
            _ssoService = ssoService;
            _tenantService = tenantService;
            _auditService = auditService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: /Auth/DelegatedLogin?token=...
        [HttpGet("/Auth/DelegatedLogin")]
        public async Task<IActionResult> DelegatedLogin(string token)
        {
            var activeTenant = _tenantService.GetCurrentTenant();
            var validation = _ssoService.ValidateDelegatedToken(token, activeTenant?.SubDomain);

            if (!validation.IsValid)
            {
                return View("DelegatedAuthError", validation.ErrorMessage);
            }

            // Provision or resolve delegated support administrator in tenant database
            var supportEmail = validation.MasterEmail;
            var supportUsername = $"support.{supportEmail.Replace("@", "_").Replace(".", "_")}";

            var user = await _userManager.FindByEmailAsync(supportEmail) 
                    ?? await _userManager.FindByNameAsync(supportUsername)
                    ?? await _userManager.FindByNameAsync(supportEmail);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = supportUsername,
                    Email = supportEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "08165680904",
                    LockoutEnabled = false
                };

                var createRes = await _userManager.CreateAsync(user, "ExwhyzeeSupport2026!*");
                if (!createRes.Succeeded)
                {
                    // Fallback to existing first SuperAdmin / Admin in database if user creation restricted
                    var existingSuper = await _userManager.FindByNameAsync("admin") ?? await _userManager.FindByNameAsync("administrator");
                    if (existingSuper != null)
                    {
                        user = existingSuper;
                    }
                }
            }

            // Ensure requested role exists
            var requestedRole = validation.RequestedRole;
            if (!string.IsNullOrWhiteSpace(requestedRole))
            {
                if (!await _roleManager.RoleExistsAsync(requestedRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(requestedRole));
                }

                if (user != null && !await _userManager.IsInRoleAsync(user, requestedRole))
                {
                    await _userManager.AddToRoleAsync(user, requestedRole);
                }
            }

            if (user != null)
            {
                // Create support claims
                var additionalClaims = new List<Claim>
                {
                    new Claim("IsMasterImpersonated", "true"),
                    new Claim("ImpersonatedBy", validation.MasterEmail),
                    new Claim("ImpersonationReason", validation.Reason),
                    new Claim("ImpersonationRole", validation.RequestedRole),
                    new Claim("ImpersonationNonce", validation.Nonce),
                    new Claim("DelegatedLoginAt", DateTime.UtcNow.ToString("O"))
                };

                await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, additionalClaims);

                await _auditService.LogAsync(
                    action: "DelegatedSsoAccept",
                    details: $"Support session established on '{activeTenant?.SchoolName}' for Master Admin '{validation.MasterEmail}' ({requestedRole}). Reason: {validation.Reason}",
                    userName: validation.MasterEmail,
                    userRole: requestedRole,
                    area: "Tenant",
                    severity: "Warning",
                    tenantSubDomain: activeTenant?.SubDomain,
                    tenantSchoolId: activeTenant?.Id);

                // Redirect to target administrative dashboard
                if (requestedRole == "SuperAdmin" || requestedRole == "SuperUser")
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "SuperUser" });
                }

                return RedirectToAction("Index", "DashBoard", new { area = "Admin" });
            }

            return View("DelegatedAuthError", "Unable to establish support session in tenant database.");
        }

        // GET: /Auth/ExitDelegatedLogin
        [HttpGet("/Auth/ExitDelegatedLogin")]
        public async Task<IActionResult> ExitDelegatedLogin()
        {
            var activeTenant = _tenantService.GetCurrentTenant();
            var impersonator = User.FindFirst("ImpersonatedBy")?.Value ?? User.Identity?.Name;

            await _auditService.LogAsync(
                action: "DelegatedSsoExit",
                details: $"Master Support session terminated on '{activeTenant?.SchoolName}' by '{impersonator}'.",
                userName: impersonator,
                area: "Tenant",
                severity: "Info",
                tenantSubDomain: activeTenant?.SubDomain,
                tenantSchoolId: activeTenant?.Id);

            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Master support session closed successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}
