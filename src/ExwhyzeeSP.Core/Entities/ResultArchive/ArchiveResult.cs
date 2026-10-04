using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;
using System.ComponentModel.DataAnnotations;


namespace ExwhyzeeSP.Core.Entities.ResultArchive
{
    public class ArchiveResult
    {
        public int Id { get; set; }

        public Session Session { get; set; }
        public int? SessionId { get; set; }

        public ClassLevel ClassLevel { get; set; }
        public int? ClassLevelId { get; set; }
    }
}
