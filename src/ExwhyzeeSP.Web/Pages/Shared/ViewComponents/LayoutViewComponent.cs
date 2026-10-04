using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Pages.Shared.ViewComponents
{
    public class LayoutViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
         
        public LayoutViewComponent(
            ApplicationDbContext context)
        {
            _context = context;
        }

 
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var xsetting = await _context.SuperSettings.FirstOrDefaultAsync();
             return View(xsetting);
        }
    }
}


