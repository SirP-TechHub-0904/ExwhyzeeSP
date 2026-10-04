using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace ExwhyzeeSP.Core.Entities.UI
{
    public class SiteHomeBodyAfterNew
    {
        public int Id { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public bool Show { get; set; }
    }
}
