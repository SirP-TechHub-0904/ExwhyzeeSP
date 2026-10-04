using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;

namespace ExwhyzeeSP.Core.Dtos
{
    public class SchoolSessionDto
    {
        public int Id { get; set; }
        public string FullSession { get; set; }
        public string Year { get; set; }
        public SessionStatus SessionStatus { get; set; }
    }
}