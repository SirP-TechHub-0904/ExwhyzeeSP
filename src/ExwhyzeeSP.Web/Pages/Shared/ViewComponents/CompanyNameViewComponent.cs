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
    public class CompanyNameViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
         
        public CompanyNameViewComponent(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public string UserInfo { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var xsetting = await _context.SuperSettings.FirstOrDefaultAsync();
            TempData["companyname"] = xsetting.CompanyName;
            TempData["CompanyWebsiteLink"] = xsetting.CompanyWebsiteLink;
            return View();
        }
    }
}


