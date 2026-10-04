using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;

namespace ExwhyzeeSP.Core.Entities.ResultArchive
{
    public class PrincipalArchive
    {
        public int Id { get; set; }
        public string PrincipalName { get; set; }
        public int? SessionId { get; set; }

        public int? PrincipalId { get; set; }
    }
}
