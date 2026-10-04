using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;

namespace ExwhyzeeSP.Core.Dtos.Api
{
    public class ApiSubjectListDto
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public int ClassLevelId { get; set; }
    }
}