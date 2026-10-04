using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Entities
{
    public class States
    {
        public int? Id { get; set; }

        public string StateName { get; set; } = string.Empty;

        public virtual ICollection<LocalGovs> LocalGov { get; set; } = new HashSet<LocalGovs>();
    }
}
