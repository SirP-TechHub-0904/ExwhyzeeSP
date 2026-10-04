using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Entities
{
    public class LocalGovs
    {
        public int Id { get; set; }
        public string LGAName { get; set; } = string.Empty;

        public int? StatesId { get; set; }
        public States? States { get; set; }
    }
}
