using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;

namespace ExwhyzeeSP.Core.Entities
{
    public class Documentary
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Description { get; set; }
        public string Role { get; set; }
    }
}
