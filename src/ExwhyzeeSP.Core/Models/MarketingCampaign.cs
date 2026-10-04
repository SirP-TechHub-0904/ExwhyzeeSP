using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExwhyzeeSP.Core.Models
{
    /// <summary>
    /// Marketing broadcast campaign with tracking and delivery statistics.
    /// </summary>
    public class MarketingCampaign
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Campaign Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        [Display(Name = "Email Subject")]
        public string Subject { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Sender Name")]
        public string SenderName { get; set; } = "Exwhyzee School Portal";

        [StringLength(150)]
        [EmailAddress]
        [Display(Name = "Sender Email")]
        public string SenderEmail { get; set; } = "marketing@exwhyzee.ng";

        [StringLength(100)]
        [Display(Name = "Target Audience Tag")]
        public string TargetTag { get; set; } = "All"; // "All", "Lagos Schools", "Prospects", etc.

        public int? MarketingTemplateId { get; set; }

        [ForeignKey("MarketingTemplateId")]
        public virtual MarketingTemplate? Template { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "Email Content (HTML)")]
        public string BodyHtml { get; set; } = string.Empty;

        public int TotalRecipients { get; set; } = 0;

        public int DeliveredCount { get; set; } = 0;

        public int FailedCount { get; set; } = 0;

        [StringLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Executing, Completed, Cancelled

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? ExecutionStartedUtc { get; set; }

        public DateTime? ExecutionCompletedUtc { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; } = "Admin";

        [StringLength(500)]
        public string? ExecutionSummary { get; set; }
    }
}
