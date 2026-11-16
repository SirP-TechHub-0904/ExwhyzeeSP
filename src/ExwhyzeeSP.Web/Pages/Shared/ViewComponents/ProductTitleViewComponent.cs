using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Web.Pages.Shared.ViewComponents
{
    public class ProductTitleViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ProductTitleViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var xsetting = await _context.SuperSettings.AsNoTracking().FirstOrDefaultAsync();
            return Content(xsetting?.ProductTitle ?? string.Empty);
        }
    }
}
