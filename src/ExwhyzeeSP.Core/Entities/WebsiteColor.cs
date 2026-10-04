using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;

namespace ExwhyzeeSP.Core.Entities
{
    public class WebsiteColor
    {
        public int Id { get; set; }
        public string ColorName {get;set;}
        public int WebsiteSettingId { get; set; }
    }
}
