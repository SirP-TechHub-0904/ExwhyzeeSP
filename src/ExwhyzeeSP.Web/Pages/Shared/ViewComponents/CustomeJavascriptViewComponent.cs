
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
    public class CustomeJavascriptViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CustomeJavascriptViewComponent(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var supersetting = await _context.DataConfigs.FirstOrDefaultAsync();
            if(supersetting.LayoutJavaScript != null) { 
            ViewBag.CustomeJavascript = supersetting.LayoutJavaScript;
            }
            return View();
        }
    }
}


