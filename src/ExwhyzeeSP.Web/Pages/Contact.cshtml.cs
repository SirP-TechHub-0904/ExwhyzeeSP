using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Services;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ISettingsService _settingsService;

        public ContactModel(ApplicationDbContext context, ISettingsService settingsService)
        {
            _context = context;
            _settingsService = settingsService;
        }

        public SuperSetting SuperSetting { get; set; }
        public Setting Setting { get; set; }

        public class ClientRequestDto
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
        }

        [BindProperty]
        public ClientRequestDto ClientRequest { get; set; } = new ClientRequestDto();

        [BindProperty]
        public int FirstNumber { get; set; }

        [BindProperty]
        public int SecondNumber { get; set; }

        [BindProperty]
        public int ResultCheck { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Setting = await _context.Settings.FirstOrDefaultAsync();
            var setting = await _settingsService.ValidateWeb(HttpContext);
            SuperSetting = setting.SuperSetting;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Setting = await _context.Settings.FirstOrDefaultAsync();
            var setting = await _settingsService.ValidateWeb(HttpContext);
            SuperSetting = setting.SuperSetting;

            TempData["success"] = "Thank you for contacting us! We will get back to you shortly.";
            return Page();
        }
    }
}
