using System;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.WebSetting
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,SubAdmin,mSuperAdmin")]
    public class DesignSettingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DesignSettingsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DataConfig DataConfig { get; set; } = new DataConfig();

        public async Task<IActionResult> OnGetAsync()
        {
            var config = await _context.DataConfigs.FirstOrDefaultAsync();
            if (config == null)
            {
                config = new DataConfig();
                _context.DataConfigs.Add(config);
                await _context.SaveChangesAsync();
            }
            DataConfig = config;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var superupdate = await _context.DataConfigs.FirstOrDefaultAsync();
            if (superupdate == null)
            {
                superupdate = new DataConfig();
                _context.DataConfigs.Add(superupdate);
            }

            superupdate.DashboardCSS = DataConfig.DashboardCSS;
            superupdate.LayoutCSS = DataConfig.LayoutCSS;
            superupdate.LoginCSS = DataConfig.LoginCSS;
            superupdate.MisDashboardCSS = DataConfig.MisDashboardCSS;
            superupdate.MisJobBoardCSS = DataConfig.MisJobBoardCSS;
            superupdate.ElectionCustomCss = DataConfig.ElectionCustomCss;
            superupdate.Configuration = DataConfig.Configuration;
            superupdate.RedirectTohttpswww = DataConfig.RedirectTohttpswww;
            superupdate.RedirectTohttps = DataConfig.RedirectTohttps;
            superupdate.LiveConfiguration = DataConfig.LiveConfiguration;
            superupdate.LayoutJavaScript = DataConfig.LayoutJavaScript;
            superupdate.MisLayoutJavaScript = DataConfig.MisLayoutJavaScript;

            superupdate.MailSender = DataConfig.MailSender;
            superupdate.OutlookServer = DataConfig.OutlookServer;
            superupdate.OutlookPort = DataConfig.OutlookPort;
            superupdate.OutlookUsername = DataConfig.OutlookUsername;
            superupdate.OutlookSecurity = DataConfig.OutlookSecurity;
            superupdate.OutlookSenderEmail = DataConfig.OutlookSenderEmail;
            superupdate.OutlookSSLEnable = DataConfig.OutlookSSLEnable;
            superupdate.OutlookUseDefaultCredentials = DataConfig.OutlookUseDefaultCredentials;

            superupdate.GoogleServer = DataConfig.GoogleServer;
            superupdate.GooglePort = DataConfig.GooglePort;
            superupdate.GoogleUsername = DataConfig.GoogleUsername;
            superupdate.GoogleSecurity = DataConfig.GoogleSecurity;
            superupdate.GoogleSenderEmail = DataConfig.GoogleSenderEmail;
            superupdate.GoogleSSLEnable = DataConfig.GoogleSSLEnable;
            superupdate.GoogleUseDefaultCredentials = DataConfig.GoogleUseDefaultCredentials;

            superupdate.WebmailServer = DataConfig.WebmailServer;
            superupdate.WebmailPort = DataConfig.WebmailPort;
            superupdate.WebmailUsername = DataConfig.WebmailUsername;
            superupdate.WebmailSecurity = DataConfig.WebmailSecurity;
            superupdate.WebmailSenderEmail = DataConfig.WebmailSenderEmail;
            superupdate.WebmailSSLEnable = DataConfig.WebmailSSLEnable;
            superupdate.WebmailUseDefaultCredentials = DataConfig.WebmailUseDefaultCredentials;
            superupdate.CCmails = DataConfig.CCmails;
            superupdate.BBmails = DataConfig.BBmails;

            await _context.SaveChangesAsync();
            TempData["success"] = "Custom CSS and Configuration saved successfully!";
            return RedirectToPage("./DesignSettings");
        }
    }
}
