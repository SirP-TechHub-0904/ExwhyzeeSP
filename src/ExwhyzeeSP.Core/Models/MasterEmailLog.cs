using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExwhyzeeSP.Core.Models
{
    /// <summary>
    /// Centralized email outbox log and content dump for all emails generated across the platform.
    /// Acts as an inspectable audit dump before external SMTP/SendGrid providers are attached.
    /// </summary>
    public class MasterEmailLog
    {
        [Key]
        public long Id { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(200)]
        public string RecipientEmail { get; set; } = string.Empty;

        [StringLength(150)]
        public string? RecipientName { get; set; }

        [Required]
        [StringLength(200)]
        public string SenderEmail { get; set; } = "noreply@exwhyzee.ng";

        [Required]
        [StringLength(300)]
        public string Subject { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string BodyHtml { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? BodyText { get; set; }

        [StringLength(60)]
        public string EmailCategory { get; set; } = "Notification"; // Onboarding, BillingInvoice, PasswordReset, SecurityAlert, Notification

        [StringLength(40)]
        public string Status { get; set; } = "LoggedAndDumped"; // LoggedAndDumped, Sent, Queued, Failed

        [StringLength(150)]
        public string? TriggeredBy { get; set; }

        [StringLength(100)]
        public string? TenantSubDomain { get; set; }

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }
    }
}
