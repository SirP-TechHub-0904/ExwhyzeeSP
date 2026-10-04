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
    public class WebTitleViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
         
        public WebTitleViewComponent(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public string UserInfo { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var DataConfig = await _context.DataConfigs.FirstOrDefaultAsync();
            
            ViewBag.DataConfigs = DataConfig;
            var xsetting = await _context.SuperSettings.FirstOrDefaultAsync();
            return View(xsetting);
        }
    }
}


