using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;

namespace ExwhyzeeSP.Core.Entities
{
    public class Help
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string HelpUrl { get; set; }
        public HelpType Type { get; set; }
        public int SortOrder { get; set; }
    }
}
