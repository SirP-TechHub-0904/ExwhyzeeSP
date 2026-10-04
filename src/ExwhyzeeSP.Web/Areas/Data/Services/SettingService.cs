using ExwhyzeeSP.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
// // // using Microsoft.AspNetCore.Http;
using ExwhyzeeSP.Core.Entities;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class SettingService : ISettingService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SettingService()
        {
        }

        public SettingService(
            ApplicationDbContext context = null,
            UserManager<ApplicationUser> userManager = null,
            RoleManager<IdentityRole> roleManager = null)
        {
            if (context != null) db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public UserManager<ApplicationUser> UserManager => _userManager ?? HttpHelper.Current?.RequestServices?.GetService<UserManager<ApplicationUser>>();
        public RoleManager<IdentityRole> RoleManager => _roleManager ?? HttpHelper.Current?.RequestServices?.GetService<RoleManager<IdentityRole>>();


        public async Task Create(Setting model)
        {

            db.Settings.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user?.UserName ?? "";
                tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added setting";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

        }

        public async Task CreateSettingValue(SettingsValue model)
        {
            db.SettingsValues.Add(model);
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user?.UserName ?? "";
                tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added setting value";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Edit(Setting models)
        {
            db.Entry(models).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user?.UserName ?? "";
                tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Edit setting";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task EditSettingValue(SettingsValue models)
        {
            db.Entry(models).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user?.UserName ?? "";
                tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Edit setting value";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<Setting> Get(int? id)
        {
            var item = await db.Settings.FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<SettingsDto> GetSetting()
        {
            var item = await db.Settings.FirstOrDefaultAsync();
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == item.ImageId);
            var output = new SettingsDto
            {
                Id = item.Id,
                SchoolName = item.SchoolName,
                SchoolInitials = item.SchoolInitials,
                SchoolAddress = item.SchoolAddress,
                SchoolAddress2 = item.SchoolAddress2,
                State = item.State,
                Country = item.Country,
                ContactEmail = item.ContactEmail,
                ContactPhoneNumber = item.ContactPhoneNumber,
                Image = img.ImageContent,
                SmsUsername = item.SmsUsername,
                SmsPassword = item.SmsPassword,
                Passmark = item.Passmark,
                PromotionByTrial = item.PromotionByTrial,
                ShowPositionOnResult = item.ShowPositionOnResult,
                ShowCumulativeResultForThirdTerm = item.ShowCumulativeResultForThirdTerm,
                EmailFrom = item.EmailFrom,
                MailHost = item.MailHost,
                Port = item.Port,
                MailUsername = item.MailUsername,
                MailPassword = item.MailPassword,
                SslEnabled = item.SslEnabled,
                AccessmentScore = item.AccessmentScore,
                ExamScore = item.ExamScore,
                DefaultEnrollmentRemark = item.DefaultEnrollmentRemark,
                AdmissionPinOption = item.AdmissionPinOption,
                EnableBatchResultPrinting = item.EnableBatchResultPrinting,
                GoogleAnalytics = item.GoogleAnalytics,
                ImageId = item.ImageId,
                IsPrimaryNursery = item.IsPrimaryNursery,
                NewsletterContent = item.NewsletterContent,
                PinValidOption = item.PinValidOption,
                PortalLink = item.PortalLink,
                PrintOutOption = item.PrintOutOption,
                PromoteAll = item.PromoteAll,
                PromotionByMathsAndEng = item.PromotionByMathsAndEng,
                ScreeningDate = item.ScreeningDate,
                ScreeningTime = item.ScreeningTime,
                ScreeningVenue = item.ScreeningVenue,
                ShowAccctOnResult = item.ShowAccctOnResult,
                ShowFeesOnResult = item.ShowFeesOnResult,
                ShowNewsletterPage = item.ShowNewsletterPage,
                WebsiteLink = item.WebsiteLink,
                EnableAssessment = item.EnableAssessment,
                CBTLink = item.CBTLink,
                EnableCBT = item.EnableCBT,
                EnableClassExercise = item.EnableClassExercise,
                EnableExamScore = item.EnableExamScore,
                EnableMultipleZoom = item.EnableMultipleZoom,
                EnablePreviewResult = item.EnablePreviewResult,
                EnableProject = item.EnableProject,
                EnableTestScore = item.EnableTestScore,
                EnableTestScore2 = item.EnableTestScore2,
                EnableZoom = item.EnableZoom,
                IskoollLink = item.IskoollLink,
                PaymentCallBackUrl = item.PaymentCallBackUrl,
                ZoomHostOne = item.ZoomHostOne,
                ZoomHostOnePass = item.ZoomHostOnePass,
                ZoomHostThree = item.ZoomHostThree,
                ZoomHostThreePass = item.ZoomHostThreePass,
                ZoomHostTwo = item.ZoomHostTwo,
                ZoomHostTwoPass = item.ZoomHostTwoPass

            };
            return output;

        }

        public async Task<SettingsValue> GetSettingValue(int? id)
        {
            var item = await db.SettingsValues.FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task DeleteSettingValue(int? id)
        {
            SettingsValue data = await db.SettingsValues.FindAsync(id);
            db.SettingsValues.Remove(data);
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user?.UserName ?? "";
                tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Delete setting value";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<List<SettingsValue>> SettingsValueList()
        {
            var settings = await db.SettingsValues.ToListAsync();
            return settings;
        }
    }
}

