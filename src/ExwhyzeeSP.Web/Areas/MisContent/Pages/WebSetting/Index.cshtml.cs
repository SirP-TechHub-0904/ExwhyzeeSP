using System.Threading.Tasks;
using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.WebSetting
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SuperSetting SuperSetting { get; set; } = new();
        public Setting Setting { get; set; } = new();
        public bool HasSuperSetting { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var ss = await _context.SuperSettings.AsNoTracking().FirstOrDefaultAsync();
            HasSuperSetting = ss != null;
            SuperSetting = ss ?? new SuperSetting();
            Setting = await _context.Settings.AsNoTracking().FirstOrDefaultAsync() ?? new Setting();

            return Page();
        }
    }
}
