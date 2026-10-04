using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Models
{
    /// <summary>
    /// Stores the audit history of database backups (direct download streams and Cloudinary cloud vaults).
    /// </summary>
    public class TenantBackupHistory
    {
        [Key]
        public long Id { get; set; }

        public int TenantSchoolId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "School Name")]
        public string SchoolName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Subdomain")]
        public string? Subdomain { get; set; }

        [Display(Name = "Backup Date & Time")]
        public DateTime BackupDate { get; set; } = DateTime.UtcNow;

        [StringLength(1000)]
        [Display(Name = "Vault / File URL")]
        public string? FileUrl { get; set; }

        [StringLength(255)]
        [Display(Name = "Backup File Name")]
        public string? FileName { get; set; }

        [StringLength(50)]
        [Display(Name = "Archive Size")]
        public string? FileSize { get; set; }

        [StringLength(100)]
        [Display(Name = "Storage Type")]
        public string BackupType { get; set; } = "Cloudinary Vault"; // "Cloudinary Vault", "Local Download", "Scheduled System"

        [StringLength(200)]
        [Display(Name = "Initiated By")]
        public string? InitiatedBy { get; set; }

        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Success"; // "Success", "Failed"

        [StringLength(500)]
        [Display(Name = "Details / Error Notes")]
        public string? Notes { get; set; }
    }
}
