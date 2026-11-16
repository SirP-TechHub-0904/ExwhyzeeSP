using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ExwhyzeeSP.Web.Pages.Shared.ViewComponents
{
    public class RedirectViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(Content(string.Empty));
        }
    }
}
