using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.Template
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,SubAdmin,mSuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SuperSetting SuperSetting { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            SuperSetting = await _context.SuperSettings.FirstOrDefaultAsync();
            if (SuperSetting == null)
            {
                SuperSetting = new SuperSetting();
            }
            return Page();
        }
    }
}
