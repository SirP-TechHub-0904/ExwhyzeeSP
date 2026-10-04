using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Models
{
    public class MasterCmsPage
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty; // e.g. "privacy-policy", "terms-and-conditions", "cookie-policy"

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; } = true;
    }
}
