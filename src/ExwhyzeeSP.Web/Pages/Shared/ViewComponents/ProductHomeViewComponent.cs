using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ExwhyzeeSP.Web.Pages.Shared.ViewComponents
{
    public class ProductHomeViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(Content(string.Empty));
        }
    }
}
