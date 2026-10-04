using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ExwhyzeeSP.Core.Entities;

namespace ExwhyzeeSP.Core.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }

    public class ExternalLoginListViewModel
    {
        public string? ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string? SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; } = new List<SelectListItem>();
        public string? ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Surname { get; set; }
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Other Name")]
        public string? OtherName { get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
        public string? Religion { get; set; }
        public string? Gender { get; set; }
        [Display(Name = "Contact Address")]
        public string? ContactAddress { get; set; }
        public string? City { get; set; }

        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Display(Name = "Local Government Area")]
        public string? LocalGov { get; set; }

        [Display(Name = "State Of Origin")]
        public string? StateOfOrigin { get; set; }

        public string? Nationality { get; set; } = "Nigerian";
        public string? RegisteredBy { get; set; }
        public int? ClassLevelId { get; set; }
        public string? Role { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
