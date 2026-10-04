using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
// using System.Web;
using ExwhyzeeSP.Core.Attributes;

namespace ExwhyzeeSP.Core.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Message { get; set; }

        [Display(Name = "Show Modal")]
        public bool ShowModal { get; set; }

        [Display(Name = "Show Marque")]
        public bool ShowMarque { get; set; }
    }
}