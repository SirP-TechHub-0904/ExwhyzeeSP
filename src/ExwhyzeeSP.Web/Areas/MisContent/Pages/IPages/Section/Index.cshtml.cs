using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.IPages.Section
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,Editor")]

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PageSection> PageSection { get;set; }

        public async Task OnGetAsync()
        {
            PageSection = await _context.PageSections
                .Include(p => p.WebPage).ToListAsync();
        }
    }
}



