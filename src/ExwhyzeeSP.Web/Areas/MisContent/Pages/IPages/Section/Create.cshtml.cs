using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.IPages.Section
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin,Editor")]

    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;

        public CreateModel(ApplicationDbContext context, IConfiguration config, IStorageService storageService)
        {
            _context = context;
            _config = config;
            _storageService = storageService;
        }


        [BindProperty]
        public IFormFile? imagefile { get; set; }


        [BindProperty]
        public IFormFile? videofile { get; set; }


        public IActionResult OnGet()
        {
        ViewData["WebPageId"] = new SelectList(_context.WebPages, "Id", "Title"); 

            return Page();
        }

        [BindProperty]
        public PageSection PageSection { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
             
            _context.PageSections.Add(PageSection);
            await _context.SaveChangesAsync(); TempData["success"] = "Successful";

            return RedirectToPage("./Index");
        }
    }
}




