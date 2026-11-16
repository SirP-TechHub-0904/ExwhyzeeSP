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
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,Manager,Admin,Editor")]

    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Setting Setting { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Setting = await _context.Settings.FirstOrDefaultAsync(m => m.Id == id);

            if (Setting == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}



