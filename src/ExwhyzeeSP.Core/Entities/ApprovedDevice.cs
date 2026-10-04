using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;

namespace ExwhyzeeSP.Core.Entities
{
    public class ApprovedDevice
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string MacAddress { get; set; }
        public string ImelNumber { get; set; }
        public string IpAddress { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? DeviceThatAddedThis { get; set; }
    }
}
