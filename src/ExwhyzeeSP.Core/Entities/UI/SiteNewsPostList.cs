using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace ExwhyzeeSP.Core.Entities.UI
{
    public class SiteNewsPostList
    {
        public int Id { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public int SortOrder { get; set; }
        public bool Show { get; set; }
        public string link { get; set; }
    }
}
