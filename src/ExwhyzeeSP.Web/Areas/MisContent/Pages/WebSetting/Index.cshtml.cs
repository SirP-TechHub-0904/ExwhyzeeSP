using ExwhyzeeSP.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.WebSetting
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SuperSetting SuperSetting { get; set; }
        public Setting Setting { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            SuperSetting = await _context.SuperSettings.FirstOrDefaultAsync();
            Setting = await _context.Settings.FirstOrDefaultAsync();

            return Page();
        }
    }
}



