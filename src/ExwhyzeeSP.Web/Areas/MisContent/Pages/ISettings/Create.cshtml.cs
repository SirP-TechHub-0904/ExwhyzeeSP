using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.ISettings
{
    //[Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,Manager,Admin,Editor")]

    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Setting Setting { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            var se = await _context.Settings.FirstOrDefaultAsync();
            if (se == null)
            {
                _context.Settings.Add(Setting);
                await _context.SaveChangesAsync(); TempData["success"] = "Successful";
            }
            return RedirectToPage("./Index");
        }
    }
}



