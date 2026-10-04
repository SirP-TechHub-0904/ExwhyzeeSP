using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; 

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.ISettings
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin,Manager,Admin,Editor")]

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Setting Setting { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
           

            Setting = await _context.Settings.FirstOrDefaultAsync();

            if (Setting == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}



