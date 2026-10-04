using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Models
{
    public class MasterPricingPlan
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Plan Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        [Display(Name = "Tagline / Subtitle")]
        public string? Subtitle { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Price Display")]
        public string PriceText { get; set; } = string.Empty; // e.g. "₦500 - ₦1,500", "Custom Quote"

        [Required]
        [MaxLength(100)]
        [Display(Name = "Billing Cycle")]
        public string BillingCycle { get; set; } = "per student / term"; // e.g. "/ student / term", "/ class / term"

        [Display(Name = "Description / Best For")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(80)]
        [Display(Name = "Badge / Highlight Text")]
        public string? BadgeText { get; set; } // e.g. "Most Popular", "Flexible", "Zero Upfront Cost"

        [Display(Name = "Mark as Most Popular (Highlighted)")]
        public bool IsPopular { get; set; }

        [MaxLength(100)]
        [Display(Name = "Button Text")]
        public string ButtonText { get; set; } = "Get Started";

        [MaxLength(250)]
        [Display(Name = "Button URL")]
        public string ButtonUrl { get; set; } = "/Demo";

        [Display(Name = "Features List (Enter one feature per line)")]
        public string Features { get; set; } = string.Empty;

        [Display(Name = "Display Sort Order")]
        public int SortOrder { get; set; } = 1;

        [Display(Name = "Active on Public Website")]
        public bool IsActive { get; set; } = true;
    }
}
