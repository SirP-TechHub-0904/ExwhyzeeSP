using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Models
{
    /// <summary>
    /// Marketing lead or institution contact imported via Excel, Word, or manual entry.
    /// </summary>
    public class MarketingContact
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(150)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [StringLength(150)]
        [Display(Name = "Contact Name")]
        public string? FullName { get; set; }

        [StringLength(50)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        [Display(Name = "School / Institution Name")]
        public string? SchoolName { get; set; }

        [StringLength(100)]
        [Display(Name = "Job Title / Role")]
        public string? RoleTitle { get; set; } // Proprietor, Principal, Head of Academics, Bursar

        [StringLength(100)]
        [Display(Name = "City / State")]
        public string? State { get; set; }

        [StringLength(100)]
        [Display(Name = "Audience Tag / Group")]
        public string Tag { get; set; } = "General"; // Prospects, Lagos Schools, Abuja Leads, Existing Clients

        [StringLength(50)]
        public string Source { get; set; } = "ExcelImport"; // ExcelImport, WordPaste, Manual, WebForm

        public bool IsSubscribed { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? LastEmailedUtc { get; set; }
    }
}
