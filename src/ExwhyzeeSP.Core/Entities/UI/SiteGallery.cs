using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace ExwhyzeeSP.Core.Entities.UI
{
    public class SiteGallery
    {
        public int Id { get; set; }
        [AllowHtml]
        public string UpperSection { get; set; }
        [AllowHtml]
        public string LowerSection { get; set; }
        public bool Show { get; set; }
    }
}
