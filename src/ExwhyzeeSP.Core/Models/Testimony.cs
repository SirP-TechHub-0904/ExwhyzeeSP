using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Core.Models
{
    public class Testimony
    {
        public long Id { get; set; }
        
        [Required]
        [Display(Name = "Author Name")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Designation / Role")]
        public string? Position { get; set; } // e.g. "Principal & Director of Studies"

        [Display(Name = "School / Institution")]
        public string? SchoolName { get; set; } // e.g. "Premier International Academy"

        public string? SortOrder { get; set; }

        public bool Show { get; set; } = true;

        public int Rating { get; set; } = 5;

        public string? ImageUrl { get; set; }
        public string? ImageKey { get; set; }

        [Required]
        [Display(Name = "Testimonial Content")]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

