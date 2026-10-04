
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Web.Pages.Shared.ViewComponents
{
    public class BlogHomeViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public BlogHomeViewComponent(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public string UserInfo { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var blog = await _context.Blogs.Where(x => x.Publish == true).OrderByDescending(x=>x.Date).ThenBy(x=>x.SortOrder).Take(6).ToListAsync();
            var setting = await _context.Settings.FirstOrDefaultAsync();
            var suppersetting = await _context.SuperSettings.FirstOrDefaultAsync();
            ViewBag.sxt = setting;
            ViewBag.suppersetting = suppersetting;
            return View(blog);
        }
    }
    
}


