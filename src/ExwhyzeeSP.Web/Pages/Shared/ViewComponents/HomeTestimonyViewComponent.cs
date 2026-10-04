
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
    public class HomeTestimonyViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public HomeTestimonyViewComponent(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var testimony = await _context.Testimonies.Where(x => x.Show == true).OrderBy(x => x.SortOrder).Take(10).ToListAsync();
            var setting = await _context.Settings.FirstOrDefaultAsync();
            var suppersetting = await _context.SuperSettings.FirstOrDefaultAsync();
            ViewBag.sxt = setting;
            ViewBag.suppersetting = suppersetting;
            return View(testimony);
        }
    }
}


