using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Master.Models;
using ExwhyzeeSP.Services;
using LoginViewModel = ExwhyzeeSP.Master.Models.LoginViewModel;
using ForgotPasswordViewModel = ExwhyzeeSP.Master.Models.ForgotPasswordViewModel;
using VerifyOtpViewModel = ExwhyzeeSP.Master.Models.VerifyOtpViewModel;
using ResetPasswordViewModel = ExwhyzeeSP.Master.Models.ResetPasswordViewModel;
using LoginWith2faViewModel = ExwhyzeeSP.Master.Models.LoginWith2faViewModel;
using AdminProfileViewModel = ExwhyzeeSP.Master.Models.AdminProfileViewModel;
using ChangePasswordViewModel = ExwhyzeeSP.Master.Models.ChangePasswordViewModel;
using TwoFactorManageViewModel = ExwhyzeeSP.Master.Models.TwoFactorManageViewModel;

namespace ExwhyzeeSP.Master.Controllers
{
    public class ForceChangePasswordViewModel
    {
        [Required(ErrorMessage = "New Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Master Portal Authentication Controller supporting standard login, 2FA verification,
    /// OTP password recovery, profile management, and audit logging.
    /// </summary>
    public class AccountController : Controller
    {
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutWindow = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan OtpValidityDuration = TimeSpan.FromMinutes(10);

        private readonly SignInManager<MasterAdminUser> _signInManager;
        private readonly UserManager<MasterAdminUser> _userManager;
        private readonly MasterDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IAuditLogService _auditService;
        private readonly IEmailLoggingService _emailLoggingService;

        public AccountController(
            SignInManager<MasterAdminUser> signInManager,
            UserManager<MasterAdminUser> userManager,
            MasterDbContext context,
            IMemoryCache cache,
            IAuditLogService auditService,
            IEmailLoggingService emailLoggingService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _cache = cache;
            _auditService = auditService;
            _emailLoggingService = emailLoggingService;
        }

        // ==============================================================================
        // 1. LOGIN & 2FA CHALLENGE
        // ==============================================================================

        [HttpGet("/Account/Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost("/Account/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var lockKey = "login_fail_" + ip;
            var failures = _cache.TryGetValue(lockKey, out int f) ? f : 0;
            if (failures >= MaxFailedAttempts)
            {
                ModelState.AddModelError(string.Empty, "Too many failed attempts. Account access is temporarily locked for 15 minutes.");
                return View(model);
            }

            var input = model.Email.Trim();
            var user = await _userManager.FindByEmailAsync(input) ?? await _userManager.FindByNameAsync(input);

            if (user == null)
            {
                _cache.Set(lockKey, failures + 1, LockoutWindow);
                ModelState.AddModelError(string.Empty, "Invalid administrator email or password.");
                return View(model);
            }

            if (!user.IsActive)
            {
                await _auditService.LogAsync(
                    action: "SuspendedLoginBlocked",
                    details: $"Login attempt blocked for suspended account '{input}'.",
                    userName: input,
                    area: "Security",
                    severity: "Warning",
                    httpContext: HttpContext);

                return RedirectToAction(nameof(Suspended), new { email = user.Email });
            }

            // Verify password
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                _cache.Set(lockKey, failures + 1, LockoutWindow);
                await _auditService.LogAsync(
                    action: "LoginFailed",
                    details: $"Invalid password attempt for account '{input}'.",
                    userName: input,
                    area: "Security",
                    severity: "Warning",
                    httpContext: HttpContext);

                ModelState.AddModelError(string.Empty, "Invalid administrator email or password.");
                return View(model);
            }

            // Check if Two-Factor Authentication is enabled
            if (user.TwoFactorEnabled)
            {
                // Generate 6-digit OTP code for 2FA
                var otp = GenerateSecure6DigitOtp();
                user.CurrentOtpHash = HashOtp(otp);
                user.OtpExpiryUtc = DateTime.UtcNow.Add(OtpValidityDuration);
                await _userManager.UpdateAsync(user);

                // Dispatch 2FA email notification
                var emailHtml = $@"
                <div style='font-family: Arial, sans-serif; max-width: 580px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 10px; padding: 28px;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h2 style='color: #0b2239; margin: 0;'>ExwhyzeeSP Master Platform</h2>
                        <span style='color: #0284c7; font-weight: bold; font-size: 13px;'>TWO-FACTOR AUTHENTICATION SECURITY CODE</span>
                    </div>
                    <p>Dear <strong>{user.FullName ?? user.UserName}</strong>,</p>
                    <p>You are signing into the ExwhyzeeSP Master Administrator Dashboard from IP <code>{ip}</code>.</p>
                    <div style='background: #f0fdf4; border: 2px dashed #22c55e; border-radius: 8px; text-align: center; padding: 20px; margin: 24px 0;'>
                        <span style='color: #166534; font-size: 12px; font-weight: bold; letter-spacing: 1px;'>YOUR 2FA VERIFICATION CODE:</span>
                        <div style='font-size: 32px; font-weight: 800; letter-spacing: 8px; color: #15803d; margin: 8px 0;'>{otp}</div>
                        <small style='color: #64748b;'>Valid for 10 minutes. Never share this code with anyone.</small>
                    </div>
                    <p style='color: #64748b; font-size: 13px;'>If you did not initiate this login request, please contact platform security immediately and change your master password.</p>
                    <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;' />
                    <small style='color: #94a3b8;'>Exwhyzee SP Master Security Engine &bull; Automated 2FA Service</small>
                </div>";

                await _emailLoggingService.LogAndDumpEmailAsync(
                    recipientEmail: user.Email!,
                    subject: $"[2FA Code: {otp}] Your ExwhyzeeSP Master Login Security Code",
                    bodyHtml: emailHtml,
                    recipientName: user.FullName,
                    category: "TwoFactorAuth",
                    senderEmail: "security@exwhyzee.ng");

                await _auditService.LogAsync(
                    action: "2fa_challenge_sent",
                    details: $"Two-factor authentication challenge dispatched to {user.Email}.",
                    userName: user.UserName,
                    area: "Security",
                    severity: "Info",
                    httpContext: HttpContext);

                return RedirectToAction(nameof(LoginWith2fa), new { userId = user.Id, rememberMe = model.RememberMe, returnUrl });
            }

            // Normal Sign-In
            await _signInManager.SignInAsync(user, model.RememberMe);
            _cache.Remove(lockKey);

            user.LastLoginUtc = DateTime.UtcNow;
            user.LastLoginIp = ip;
            await _userManager.UpdateAsync(user);

            await _auditService.LogAsync(
                action: "LoginSuccess",
                details: $"Master user '{user.UserName}' logged in successfully ({user.RoleTitle ?? "SuperAdmin"}).",
                userName: user.UserName,
                userRole: user.RoleTitle,
                area: "Security",
                severity: "Info",
                httpContext: HttpContext);

            if (user.RequirePasswordChange)
            {
                return RedirectToAction(nameof(ForceChangePassword));
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Account/LoginWith2fa
        [HttpGet("/Account/LoginWith2fa")]
        public async Task<IActionResult> LoginWith2fa(string userId, bool rememberMe = false, string? returnUrl = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid authentication session. Please log in again.";
                return RedirectToAction(nameof(Login));
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginWith2faViewModel
            {
                UserId = user.Id,
                Email = MaskEmail(user.Email ?? ""),
                RememberMe = rememberMe,
                Channel = user.TwoFactorChannel ?? "Email"
            });
        }

        // POST: /Account/LoginWith2fa
        [HttpPost("/Account/LoginWith2fa")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User session expired. Please sign in again.";
                return RedirectToAction(nameof(Login));
            }

            var cleanCode = model.TwoFactorCode.Trim().Replace(" ", "").Replace("-", "");

            if (user.OtpExpiryUtc == null || user.OtpExpiryUtc < DateTime.UtcNow || string.IsNullOrWhiteSpace(user.CurrentOtpHash))
            {
                ModelState.AddModelError(string.Empty, "Security code has expired. Please log in again to generate a fresh OTP code.");
                return View(model);
            }

            if (user.CurrentOtpHash != HashOtp(cleanCode))
            {
                await _auditService.LogAsync(
                    action: "2fa_failed_attempt",
                    details: $"Invalid 2FA code entered for user '{user.UserName}'.",
                    userName: user.UserName,
                    area: "Security",
                    severity: "Warning",
                    httpContext: HttpContext);

                ModelState.AddModelError(string.Empty, "Invalid 6-digit security code. Please check and try again.");
                return View(model);
            }

            // Clear OTP upon successful verification
            user.CurrentOtpHash = null;
            user.OtpExpiryUtc = null;
            user.LastLoginUtc = DateTime.UtcNow;
            user.LastLoginIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            await _userManager.UpdateAsync(user);

            await _signInManager.SignInAsync(user, model.RememberMe);

            await _auditService.LogAsync(
                action: "2fa_verified_success",
                details: $"Two-factor authentication successfully verified for '{user.UserName}'.",
                userName: user.UserName,
                userRole: user.RoleTitle,
                area: "Security",
                severity: "Info",
                httpContext: HttpContext);

            if (user.RequirePasswordChange)
            {
                return RedirectToAction(nameof(ForceChangePassword));
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }

        // ==============================================================================
        // 2. PASSWORD RESET VIA 6-DIGIT EMAIL OTP
        // ==============================================================================

        // GET: /Account/ForgotPassword
        [HttpGet("/Account/ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        // POST: /Account/ForgotPassword
        [HttpPost("/Account/ForgotPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var cleanEmail = model.Email.Trim();
            var user = await _userManager.FindByEmailAsync(cleanEmail);

            if (user != null && user.IsActive)
            {
                var otp = GenerateSecure6DigitOtp();
                user.CurrentOtpHash = HashOtp(otp);
                user.OtpExpiryUtc = DateTime.UtcNow.Add(OtpValidityDuration);
                await _userManager.UpdateAsync(user);

                var emailHtml = $@"
                <div style='font-family: Arial, sans-serif; max-width: 580px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 10px; padding: 28px;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h2 style='color: #0b2239; margin: 0;'>ExwhyzeeSP Master Platform</h2>
                        <span style='color: #0284c7; font-weight: bold; font-size: 13px;'>PASSWORD RESET ONE-TIME PASSWORD (OTP)</span>
                    </div>
                    <p>Dear <strong>{user.FullName ?? user.UserName}</strong>,</p>
                    <p>We received a request to reset your Master Administrator password.</p>
                    <div style='background: #eff6ff; border: 2px dashed #3b82f6; border-radius: 8px; text-align: center; padding: 20px; margin: 24px 0;'>
                        <span style='color: #1e40af; font-size: 12px; font-weight: bold; letter-spacing: 1px;'>YOUR PASSWORD RESET OTP:</span>
                        <div style='font-size: 32px; font-weight: 800; letter-spacing: 8px; color: #1d4ed8; margin: 8px 0;'>{otp}</div>
                        <small style='color: #64748b;'>Expires in 10 minutes. Do not share with anyone.</small>
                    </div>
                    <p style='color: #64748b; font-size: 13px;'>If you did not request a password reset, please ignore this email or notify security immediately.</p>
                    <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;' />
                    <small style='color: #94a3b8;'>Exwhyzee SP Master Platform &bull; Security &amp; Audit Engine</small>
                </div>";

                await _emailLoggingService.LogAndDumpEmailAsync(
                    recipientEmail: user.Email!,
                    subject: $"[Reset OTP: {otp}] ExwhyzeeSP Master Password Reset Code",
                    bodyHtml: emailHtml,
                    recipientName: user.FullName,
                    category: "PasswordReset",
                    senderEmail: "security@exwhyzee.ng");

                await _auditService.LogAsync(
                    action: "PasswordResetOtpRequested",
                    details: $"Password reset OTP generated and dispatched to {user.Email}.",
                    userName: user.UserName,
                    area: "Security",
                    severity: "Info",
                    httpContext: HttpContext);
            }

            // Always redirect to VerifyOtp to prevent email enumeration
            return RedirectToAction(nameof(VerifyOtp), new { email = cleanEmail });
        }

        // GET: /Account/VerifyOtp
        [HttpGet("/Account/VerifyOtp")]
        public IActionResult VerifyOtp(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction(nameof(ForgotPassword));

            return View(new VerifyOtpViewModel { Email = email });
        }

        // POST: /Account/VerifyOtp
        [HttpPost("/Account/VerifyOtp")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid request. Please restart password recovery.");
                return View(model);
            }

            var cleanOtp = model.OtpCode.Trim();

            if (user.OtpExpiryUtc == null || user.OtpExpiryUtc < DateTime.UtcNow || string.IsNullOrWhiteSpace(user.CurrentOtpHash))
            {
                ModelState.AddModelError(string.Empty, "OTP has expired. Please request a new code.");
                return View(model);
            }

            if (user.CurrentOtpHash != HashOtp(cleanOtp))
            {
                await _auditService.LogAsync(
                    action: "OtpVerificationFailed",
                    details: $"Invalid OTP entered for password reset on '{user.Email}'.",
                    userName: user.UserName,
                    area: "Security",
                    severity: "Warning",
                    httpContext: HttpContext);

                ModelState.AddModelError(string.Empty, "Incorrect OTP code. Please verify the 6 digits from your email.");
                return View(model);
            }

            // Generate verified Identity password reset token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Invalidate OTP so it cannot be reused
            user.CurrentOtpHash = null;
            user.OtpExpiryUtc = null;
            await _userManager.UpdateAsync(user);

            await _auditService.LogAsync(
                action: "OtpVerifiedSuccess",
                details: $"Password reset OTP verified successfully for '{user.Email}'.",
                userName: user.UserName,
                area: "Security",
                severity: "Info",
                httpContext: HttpContext);

            return RedirectToAction(nameof(ResetPassword), new { email = user.Email, token = resetToken });
        }

        // GET: /Account/ResetPassword
        [HttpGet("/Account/ResetPassword")]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                TempData["ErrorMessage"] = "Invalid or expired password reset link.";
                return RedirectToAction(nameof(Login));
            }

            return View(new ResetPasswordViewModel { Email = email, ResetToken = token });
        }

        // POST: /Account/ResetPassword
        [HttpPost("/Account/ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User account not found.";
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.NewPassword);
            if (result.Succeeded)
            {
                await _auditService.LogAsync(
                    action: "PasswordResetSuccess",
                    details: $"Administrator password was successfully reset via OTP verification.",
                    userName: user.UserName,
                    userRole: user.RoleTitle,
                    area: "Security",
                    severity: "Info",
                    httpContext: HttpContext);

                TempData["SuccessMessage"] = "Your password has been successfully reset! You can now log in with your new password.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // ==============================================================================
        // 3. MASTER HEADER ACCOUNT SUITE (PROFILE, PASSWORD, 2FA, AUDIT)
        // ==============================================================================

        // GET: /Account/Profile & /Admin/Profile
        [Authorize]
        [HttpGet("/Account/Profile")]
        [HttpGet("/Admin/Profile")]
        [HttpGet("/Admin/Account/Profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new AdminProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                RoleTitle = user.RoleTitle ?? "SuperAdmin",
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedDate = user.CreatedDate,
                LastLoginUtc = user.LastLoginUtc,
                LastLoginIp = user.LastLoginIp
            };

            return View("~/Views/Account/Profile.cshtml", model);
        }

        // POST: /Account/Profile
        [Authorize]
        [HttpPost("/Account/Profile")]
        [HttpPost("/Admin/Profile")]
        [HttpPost("/Admin/Account/Profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(AdminProfileViewModel model)
        {
            if (!ModelState.IsValid) return View("~/Views/Account/Profile.cshtml", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FullName = model.FullName.Trim();
            user.PhoneNumber = model.PhoneNumber?.Trim();
            user.ProfileImageUrl = model.ProfileImageUrl?.Trim();

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _auditService.LogAsync(
                    action: "ProfileUpdated",
                    details: $"Administrator updated profile information.",
                    userName: user.UserName,
                    area: "Security",
                    severity: "Info",
                    httpContext: HttpContext);

                TempData["SuccessMessage"] = "Profile details updated successfully.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("~/Views/Account/Profile.cshtml", model);
        }

        // GET: /Account/ChangePassword & /Admin/ChangePassword
        [Authorize]
        [HttpGet("/Account/ChangePassword")]
        [HttpGet("/Admin/ChangePassword")]
        [HttpGet("/Admin/Account/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View("~/Views/Account/ChangePassword.cshtml", new ChangePasswordViewModel());
        }

        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost("/Account/ChangePassword")]
        [HttpPost("/Admin/ChangePassword")]
        [HttpPost("/Admin/Account/ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View("~/Views/Account/ChangePassword.cshtml", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);

                await _auditService.LogAsync(
                    action: "PasswordChanged",
                    details: $"Administrator changed account password.",
                    userName: user.UserName,
                    area: "Security",
                    severity: "Info",
                    httpContext: HttpContext);

                TempData["SuccessMessage"] = "Your password has been changed successfully.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("~/Views/Account/ChangePassword.cshtml", model);
        }

        // GET: /Account/TwoFactor & /Admin/TwoFactor
        [Authorize]
        [HttpGet("/Account/TwoFactor")]
        [HttpGet("/Admin/TwoFactor")]
        [HttpGet("/Admin/Account/TwoFactor")]
        public async Task<IActionResult> TwoFactor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var email = user.Email ?? user.UserName ?? "admin@exwhyzee.ng";
            var qrUri = $"otpauth://totp/ExwhyzeeSP:{Uri.EscapeDataString(email)}?secret={unformattedKey}&issuer=ExwhyzeeSP&digits=6";

            var model = new TwoFactorManageViewModel
            {
                Is2faEnabled = user.TwoFactorEnabled,
                PreferredChannel = user.TwoFactorChannel ?? "Email",
                AuthenticatorKey = FormatAuthenticatorKey(unformattedKey ?? ""),
                QrCodeUri = qrUri,
                UserEmail = email
            };

            return View("~/Views/Account/TwoFactor.cshtml", model);
        }

        private static string FormatAuthenticatorKey(string unformattedKey)
        {
            if (string.IsNullOrEmpty(unformattedKey)) return "";
            var result = new System.Text.StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }
            return result.ToString().ToUpperInvariant();
        }

        // POST: /Account/ToggleTwoFactor
        [Authorize]
        [HttpPost("/Account/ToggleTwoFactor")]
        [HttpPost("/Admin/ToggleTwoFactor")]
        [HttpPost("/Admin/Account/ToggleTwoFactor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTwoFactor(bool enable, string channel = "Email", string? verificationCode = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Detect if no state change is occurring for Email channel
            if (enable && user.TwoFactorEnabled && string.Equals(user.TwoFactorChannel, channel, StringComparison.OrdinalIgnoreCase) && channel == "Email")
            {
                TempData["InfoMessage"] = "Email One-Time Code (OTP) is already your active 2FA channel. No changes were necessary.";
                return RedirectToAction(nameof(TwoFactor));
            }

            if (enable && (channel == "Google" || channel == "Microsoft"))
            {
                if (string.IsNullOrWhiteSpace(verificationCode))
                {
                    TempData["ErrorMessage"] = $"Please enter the 6-digit confirmation code generated by your {channel} Authenticator app before saving.";
                    return RedirectToAction(nameof(TwoFactor));
                }

                var cleanCode = verificationCode.Trim().Replace(" ", "").Replace("-", "");
                var isTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, cleanCode);
                if (!isTokenValid)
                {
                    TempData["ErrorMessage"] = $"Invalid {channel} Authenticator verification code. Please check that your device clock is synchronized and enter the current 6-digit code.";
                    return RedirectToAction(nameof(TwoFactor));
                }
            }

            user.TwoFactorEnabled = enable;
            user.TwoFactorChannel = enable ? channel : null;
            await _userManager.UpdateAsync(user);

            var channelDisplayName = channel switch
            {
                "Google" => "Google Authenticator",
                "Microsoft" => "Microsoft Authenticator",
                _ => "Email OTP"
            };

            await _auditService.LogAsync(
                action: enable ? "2fa_enabled" : "2fa_disabled",
                details: $"Two-Factor Authentication was {(enable ? "ENABLED (" + channelDisplayName + ")" : "DISABLED")} by '{user.UserName}'.",
                userName: user.UserName,
                area: "Security",
                severity: "Warning",
                httpContext: HttpContext);

            TempData["SuccessMessage"] = $"Two-Factor Authentication has been {(enable ? "ENABLED via " + channelDisplayName : "DISABLED")}.";
            return RedirectToAction(nameof(TwoFactor));
        }

        // GET: /Account/ActivityLog & /Admin/ActivityLog & /Admin/Account/ActivityLog
        [Authorize]
        [HttpGet("/Account/ActivityLog")]
        [HttpGet("/Admin/ActivityLog")]
        [HttpGet("/Admin/Account/ActivityLog")]
        public async Task<IActionResult> ActivityLog(
            string? search,
            string? actionType,
            string? severity,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 15)
        {
            if (page < 1) page = 1;
            if (pageSize < 5) pageSize = 15;
            if (pageSize > 100) pageSize = 100;

            var userName = User.Identity?.Name ?? "";
            var isSuperAdmin = User.IsInRole("SuperAdmin");

            // Base query: superadmins can see all system security logs or their own; by default all user logs
            var baseQuery = _context.AuditLogs.AsNoTracking().AsQueryable();

            if (!isSuperAdmin)
            {
                baseQuery = baseQuery.Where(l => l.UserName == userName);
            }

            // Filtered Query with search, actionType, severity, and date range
            var query = baseQuery;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(l =>
                    l.Action.Contains(search) ||
                    l.IpAddress.Contains(search) ||
                    (l.Details != null && l.Details.Contains(search)) ||
                    (l.Browser != null && l.Browser.Contains(search)) ||
                    (l.DeviceType != null && l.DeviceType.Contains(search)) ||
                    (l.OperatingSystem != null && l.OperatingSystem.Contains(search)) ||
                    (l.UserName != null && l.UserName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(actionType))
            {
                query = query.Where(l => l.Action == actionType || l.Action.Contains(actionType));
            }

            if (!string.IsNullOrWhiteSpace(severity))
            {
                query = query.Where(l => l.Severity == severity);
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

            // Scoped Summary Statistics across the active time frame & filters
            var totalAllLogs = await query.CountAsync();
            var totalLogins = await query.CountAsync(l => l.Action.Contains("Login") || l.Action.Contains("SignIn"));
            var totalSso = await query.CountAsync(l => l.Action.Contains("SSO") || l.Action.Contains("Delegated"));
            var totalWarnings = await query.CountAsync(l => l.Severity == "Warning" || l.Severity == "Critical" || l.Action.Contains("Failed"));
            var uniqueIps = await query.Select(l => l.IpAddress).Distinct().CountAsync();

            // Dynamic Activity Trend for ApexCharts across selected timeframe
            DateTime trendStart;
            DateTime trendEnd;

            if (fromDate.HasValue && toDate.HasValue)
            {
                trendStart = fromDate.Value.Date;
                trendEnd = toDate.Value.Date;
                if (trendEnd < trendStart) (trendStart, trendEnd) = (trendEnd, trendStart);
                // Cap span to 60 days to prevent excessive rendering
                if ((trendEnd - trendStart).TotalDays > 60)
                {
                    trendStart = trendEnd.AddDays(-60);
                }
            }
            else if (fromDate.HasValue)
            {
                trendStart = fromDate.Value.Date;
                trendEnd = DateTime.UtcNow.Date;
                if ((trendEnd - trendStart).TotalDays > 60) trendStart = trendEnd.AddDays(-60);
            }
            else if (toDate.HasValue)
            {
                trendEnd = toDate.Value.Date;
                trendStart = trendEnd.AddDays(-13);
            }
            else
            {
                // Default rolling 14-day window
                trendEnd = DateTime.UtcNow.Date;
                trendStart = trendEnd.AddDays(-13);
            }

            var recentLogs = await query
                .Where(l => l.TimestampUtc >= trendStart && l.TimestampUtc <= trendEnd.AddDays(1).AddTicks(-1))
                .Select(l => new { l.TimestampUtc, l.Action })
                .ToListAsync();

            var trendDates = new List<string>();
            var trendLogins = new List<int>();
            var trendSso = new List<int>();
            var trendOther = new List<int>();

            var totalTrendDays = Math.Max(1, (int)(trendEnd - trendStart).TotalDays + 1);
            for (int i = 0; i < totalTrendDays; i++)
            {
                var targetDay = trendStart.AddDays(i);
                var dayStr = targetDay.ToString("MMM dd");
                trendDates.Add(dayStr);

                var dayLogs = recentLogs.Where(l => l.TimestampUtc.Date == targetDay.Date).ToList();
                trendLogins.Add(dayLogs.Count(l => l.Action.Contains("Login") || l.Action.Contains("SignIn")));
                trendSso.Add(dayLogs.Count(l => l.Action.Contains("SSO") || l.Action.Contains("Delegated")));
                trendOther.Add(dayLogs.Count(l => !l.Action.Contains("Login") && !l.Action.Contains("SignIn") && !l.Action.Contains("SSO") && !l.Action.Contains("Delegated")));
            }

            var totalItems = totalAllLogs;

            var logs = await query
                .OrderByDescending(l => l.TimestampUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            MasterAdminUser? auditedUser = null;
            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim();
                auditedUser = await _userManager.FindByEmailAsync(cleanSearch)
                           ?? await _userManager.FindByNameAsync(cleanSearch)
                           ?? await _context.Users.FirstOrDefaultAsync(u => u.Id == cleanSearch || u.Email == cleanSearch || u.UserName == cleanSearch);
            }

            ViewBag.Search = search;
            ViewBag.AuditedUser = auditedUser;
            ViewBag.ActionType = actionType;
            ViewBag.Severity = severity;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewBag.TotalAllLogs = totalAllLogs;
            ViewBag.TotalLogins = totalLogins;
            ViewBag.TotalSso = totalSso;
            ViewBag.TotalWarnings = totalWarnings;
            ViewBag.UniqueIps = uniqueIps;

            ViewBag.TrendDates = trendDates;
            ViewBag.TrendLogins = trendLogins;
            ViewBag.TrendSso = trendSso;
            ViewBag.TrendOther = trendOther;

            ViewBag.AvailableActions = await baseQuery
                .Select(l => l.Action)
                .Distinct()
                .Take(25)
                .ToListAsync();

            return View("~/Views/Account/ActivityLog.cshtml", logs);
        }

        // POST: /Admin/Account/PurgeActivityLogs
        [Authorize]
        [HttpPost("/Admin/Account/PurgeActivityLogs")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurgeActivityLogs(int? daysOlderThan, DateTime? purgeFromDate, DateTime? purgeToDate, string? confirmText)
        {
            if (confirmText != "PURGE")
            {
                TempData["ErrorMessage"] = "Confirmation mismatch. You must type 'PURGE' in capital letters to confirm database cleanup.";
                return RedirectToAction(nameof(ActivityLog));
            }

            var query = _context.AuditLogs.AsQueryable();

            if (daysOlderThan.HasValue && daysOlderThan.Value > 0)
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-Math.Abs(daysOlderThan.Value));
                query = query.Where(l => l.TimestampUtc < cutoffDate);
            }
            else if (purgeFromDate.HasValue && purgeToDate.HasValue)
            {
                var endOfPurgeDay = purgeToDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(l => l.TimestampUtc >= purgeFromDate.Value.Date && l.TimestampUtc <= endOfPurgeDay);
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid purge parameters specified. Please select a retention period or date range.";
                return RedirectToAction(nameof(ActivityLog));
            }

            var toDelete = await query.ToListAsync();
            var count = toDelete.Count;

            if (count > 0)
            {
                _context.AuditLogs.RemoveRange(toDelete);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    action: "ActivityLogsPurged",
                    details: $"Master administrator purged {count:N0} security activity log entries to reclaim storage.",
                    userName: User.Identity?.Name,
                    area: "Security",
                    severity: "Warning",
                    httpContext: HttpContext);

                TempData["SuccessMessage"] = $"Successfully purged {count:N0} activity log records from the database.";
            }
            else
            {
                TempData["InfoMessage"] = "No activity logs matched the specified purge criteria.";
            }

            return RedirectToAction(nameof(ActivityLog));
        }

        // GET: /Admin/Account/ExportActivityLogCsv
        [Authorize]
        [HttpGet("/Admin/Account/ExportActivityLogCsv")]
        public async Task<IActionResult> ExportActivityLogCsv(
            string? search,
            string? actionType,
            string? severity,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.AuditLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(l =>
                    l.Action.Contains(search) ||
                    l.IpAddress.Contains(search) ||
                    (l.Details != null && l.Details.Contains(search)) ||
                    (l.UserName != null && l.UserName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(actionType))
            {
                query = query.Where(l => l.Action == actionType || l.Action.Contains(actionType));
            }

            if (!string.IsNullOrWhiteSpace(severity))
            {
                query = query.Where(l => l.Severity == severity);
            }

            if (fromDate.HasValue) query = query.Where(l => l.TimestampUtc >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(l => l.TimestampUtc <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            var logs = await query
                .OrderByDescending(l => l.TimestampUtc)
                .Take(10000)
                .ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("EventId,TimestampUtc,Actor,Role,Action,Area,Severity,IpAddress,Browser,Device,OperatingSystem,TenantSubDomain,Details");

            foreach (var l in logs)
            {
                var cleanDetails = (l.Details ?? "").Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", "");
                builder.AppendLine($"\"{l.Id}\",\"{l.TimestampUtc:yyyy-MM-dd HH:mm:ss}\",\"{l.UserName}\",\"{l.UserRole}\",\"{l.Action}\",\"{l.Area}\",\"{l.Severity}\",\"{l.IpAddress}\",\"{l.Browser}\",\"{l.DeviceType}\",\"{l.OperatingSystem}\",\"{l.TenantSubDomain}\",\"{cleanDetails}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            return File(bytes, "text/csv", $"Security_ActivityLog_Statement_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }

        // GET: /Admin/Account/ActivityStatement (Official Regulatory / Audit Statement Printable View)
        [Authorize]
        [HttpGet("/Admin/Account/ActivityStatement")]
        [HttpGet("/Account/ActivityStatement")]
        public async Task<IActionResult> ActivityStatement(
            string? search,
            string? actionType,
            string? severity,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.AuditLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(l =>
                    l.Action.Contains(search) ||
                    l.IpAddress.Contains(search) ||
                    (l.Details != null && l.Details.Contains(search)) ||
                    (l.UserName != null && l.UserName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(actionType))
            {
                query = query.Where(l => l.Action == actionType || l.Action.Contains(actionType));
            }

            if (!string.IsNullOrWhiteSpace(severity))
            {
                query = query.Where(l => l.Severity == severity);
            }

            if (fromDate.HasValue) query = query.Where(l => l.TimestampUtc >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(l => l.TimestampUtc <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            var logs = await query
                .OrderByDescending(l => l.TimestampUtc)
                .Take(2500)
                .ToListAsync();

            MasterAdminUser? auditedUser = null;
            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim();
                auditedUser = await _userManager.FindByEmailAsync(cleanSearch)
                           ?? await _userManager.FindByNameAsync(cleanSearch)
                           ?? await _context.Users.FirstOrDefaultAsync(u => u.Id == cleanSearch || u.Email == cleanSearch || u.UserName == cleanSearch);
            }

            ViewBag.Search = search;
            ViewBag.AuditedUser = auditedUser;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd") ?? logs.LastOrDefault()?.TimestampUtc.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd");
            ViewBag.GeneratedAt = DateTime.UtcNow;
            ViewBag.GeneratedBy = User.Identity?.Name ?? "SuperAdmin";
            ViewBag.StatementRef = "EXZ-SEC-STMT-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            return View("~/Views/Account/ActivityStatement.cshtml", logs);
        }

        // ==============================================================================
        // 4. LOGOUT & ACCESS DENIED
        // ==============================================================================

        [Authorize]
        [HttpPost("/Account/Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = User.Identity?.Name;
            await _auditService.LogAsync(
                action: "Logout",
                details: "Master administrator signed out.",
                userName: user,
                area: "Security",
                severity: "Info",
                httpContext: HttpContext);

            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("/Account/AccessDenied")]
        public IActionResult AccessDenied() => View();

        // ==============================================================================
        // 4B. SUSPENDED ACCOUNT NOTICE & FORCED PASSWORD CHANGE
        // ==============================================================================

        [HttpGet("/Account/Suspended")]
        [AllowAnonymous]
        public IActionResult Suspended(string? email = null)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpGet("/Account/ForceChangePassword")]
        [Authorize]
        public IActionResult ForceChangePassword()
        {
            return View(new ForceChangePasswordViewModel());
        }

        [HttpPost("/Account/ForceChangePassword")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceChangePassword(ForceChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                user.RequirePasswordChange = false;
                await _userManager.UpdateAsync(user);

                await _auditService.LogAsync(
                    action: "PasswordChanged_ForcedOnFirstLogin",
                    details: $"User '{user.UserName}' completed mandatory initial password change.",
                    userName: user.UserName,
                    area: "Security",
                    severity: "Info",
                    httpContext: HttpContext);

                TempData["SuccessMessage"] = "Your password has been successfully established! Welcome to ExwhyzeeSP Master.";
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return View(model);
        }

        // ==============================================================================
        // 5. HELPER METHODS
        // ==============================================================================

        private static string GenerateSecure6DigitOtp()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000).ToString("D6");
        }

        private static string HashOtp(string otp)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp + "ExwhyzeeSP_SecureOtpSalt_2026"));
            return Convert.ToHexString(bytes);
        }

        private static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) return email;
            var parts = email.Split('@');
            var name = parts[0];
            var maskedName = name.Length > 2 
                ? name.Substring(0, 2) + new string('*', name.Length - 2) 
                : name + "***";
            return $"{maskedName}@{parts[1]}";
        }
    }
}
