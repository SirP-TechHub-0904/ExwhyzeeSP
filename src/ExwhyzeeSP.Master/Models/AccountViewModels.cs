using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Master.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email address or username is required")]
        [Display(Name = "Admin Email or Username")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember this device for 30 days")]
        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter your administrator email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Registered Admin Email")]
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyOtpViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the 6-digit OTP code")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be exactly 6 digits")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "OTP must consist of 6 numeric digits")]
        [Display(Name = "6-Digit Security OTP")]
        public string OtpCode { get; set; } = string.Empty;

        public string Purpose { get; set; } = "PasswordReset"; // "PasswordReset" or "TwoFactorLogin"
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string ResetToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginWith2faViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the 6-digit security code")]
        [StringLength(7, MinimumLength = 6, ErrorMessage = "Code must be 6 digits")]
        [Display(Name = "Security Verification Code")]
        public string TwoFactorCode { get; set; } = string.Empty;

        [Display(Name = "Remember this browser for 30 days")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }

        public string Channel { get; set; } = "Email"; // "Email" or "Authenticator"
    }

    public class AdminProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(150)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Official Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone / WhatsApp Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Role Title")]
        public string RoleTitle { get; set; } = "SuperAdmin";

        [Display(Name = "Profile Photo URL / Path")]
        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginUtc { get; set; }
        public string? LastLoginIp { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Secure Password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class TwoFactorManageViewModel
    {
        public bool Is2faEnabled { get; set; }
        public string PreferredChannel { get; set; } = "Email"; // "Email", "Authenticator"
        public string? AuthenticatorKey { get; set; }
        public string? QrCodeUri { get; set; }
        public string UserEmail { get; set; } = string.Empty;
    }
}
