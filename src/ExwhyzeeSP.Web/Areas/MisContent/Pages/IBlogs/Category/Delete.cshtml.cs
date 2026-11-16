using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.IBlogs.Category
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,Editor")]

    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BlogCategory BlogCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BlogCategory = await _context.BlogCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (BlogCategory == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BlogCategory = await _context.BlogCategories.FindAsync(id);

            if (BlogCategory != null)
            {
                _context.BlogCategories.Remove(BlogCategory);
                await _context.SaveChangesAsync(); TempData["success"] = "Successful";
            }

            return RedirectToPage("./Index");
        }
    }
}



