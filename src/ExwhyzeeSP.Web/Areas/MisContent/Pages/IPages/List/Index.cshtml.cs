using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.IPages.List
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,Editor")]

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PageSectionList> PageSectionList { get;set; }

        public async Task OnGetAsync()
        {
            PageSectionList = await _context.PageSectionLists
                .Include(p => p.PageSection)
                .Include(p => p.PageSection)

                .ToListAsync();
        }
    }
}



