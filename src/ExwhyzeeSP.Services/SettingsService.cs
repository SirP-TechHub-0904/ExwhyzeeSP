using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ApplicationDbContext _context;

        public SettingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationWebDto> ValidateWeb(HttpContext httpContext)
        {
            VerificationWebDto response = new VerificationWebDto();
            var setting = await _context.Settings.FirstOrDefaultAsync();
            var superSetting = await _context.SuperSettings.FirstOrDefaultAsync();

            if (superSetting == null)
            {
                // Fallback default super setting if none exists yet
                superSetting = new SuperSetting
                {
                    WebsiteTitle = "School Portal",
                    TemplateLayoutKey = "Layout11111111", // Default Education layout
                    CompanyDescription = "Welcome to our school portal"
                };
            }

            response.SuperSetting = superSetting;
            response.Setting = setting;
            response.SettingFound = true;
            return response;
        }

        public async Task<SuperSetting> GetSuperSettings()
        {
            var superSetting = await _context.SuperSettings.FirstOrDefaultAsync();
            if (superSetting == null)
            {
                superSetting = new SuperSetting
                {
                    WebsiteTitle = "School Portal",
                    TemplateLayoutKey = "Layout11111111",
                    CompanyDescription = "Welcome to our school portal"
                };
            }
            return superSetting;
        }

        public async Task<bool> UseHttpsWWW()
        {
            var config = await _context.DataConfigs.FirstOrDefaultAsync();
            return config?.RedirectTohttpswww ?? false;
        }

        public async Task<bool> UseHttps()
        {
            var config = await _context.DataConfigs.FirstOrDefaultAsync();
            return config?.RedirectTohttps ?? false;
        }
    }
}
