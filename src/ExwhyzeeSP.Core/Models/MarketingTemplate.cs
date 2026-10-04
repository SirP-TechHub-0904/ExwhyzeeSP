using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExwhyzeeSP.Core.Models
{
    /// <summary>
    /// Reusable responsive HTML email marketing templates with dynamic merge tags ({{FullName}}, {{SchoolName}}, {{Email}}).
    /// </summary>
    public class MarketingTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Template Name")]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        [Display(Name = "Email Subject")]
        public string Subject { get; set; } = string.Empty;

        [StringLength(250)]
        [Display(Name = "Preheader / Preview Text")]
        public string? PreheaderText { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "HTML Body Content")]
        public string BodyHtml { get; set; } = string.Empty;

        [StringLength(60)]
        [Display(Name = "Category")]
        public string Category { get; set; } = "ProductPitch"; // ProductPitch, SpecialOffer, Announcement, Newsletter, SummitInvite

        [StringLength(300)]
        public string? Description { get; set; }

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedUtc { get; set; }
    }
}
