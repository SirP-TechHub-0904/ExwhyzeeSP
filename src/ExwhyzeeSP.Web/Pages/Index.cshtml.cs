using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ISettingsService _settingsService;

        public IndexModel(ApplicationDbContext context, ISettingsService settingsService)
        {
            _context = context;
            _settingsService = settingsService;
        }

        public string Templatechoose { get; set; }
        public SuperSetting SuperSetting { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var setting = await _settingsService.ValidateWeb(HttpContext);
            SuperSetting = setting.SuperSetting;
            Templatechoose = SuperSetting?.TemplateLayoutKey ?? "Layout11111111";

            return Page();
        }
    }
}
