using ExwhyzeeSP.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
// // // using Microsoft.AspNetCore.Http;
using ExwhyzeeSP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class ContactUsService : IContactUsService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContactUsService()
        {
        }

        public ContactUsService(
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
        public async Task Create(ContactUs model)
        {
            model.messageStatus = MessageStatus.NotReplied;
            model.DateCreated = DateTime.UtcNow.AddHours(1);
            db.ContactUs.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            //var userId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
            //Tracker tracker = new Tracker();
            //tracker.UserId = userId;
            //tracker.UserName = user?.UserName ?? "";
            //tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
            //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            //tracker.Note = tracker.FullName + " " + "Contact Us";
            ////db.Trackers.Add(tracker);
            //await db.SaveChangesAsync();
        }

        public async Task Reply(MessageReply model, int id)
        {
            model.MessageId = id;
            model.ReplyDate = DateTime.UtcNow.AddHours(1);
            db.MessageReply.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            //var userId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
            //Tracker tracker = new Tracker();
            //tracker.UserId = userId;
            //tracker.UserName = user?.UserName ?? "";
            //tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
            //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            //tracker.Note = tracker.FullName + " " + "Replied to a message";
            ////db.Trackers.Add(tracker);
            //await db.SaveChangesAsync();
        }

        public Task Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public async Task<ContactUs> Get(int? id)
        {
            var item = await db.ContactUs.Include(x => x.MessageReply).FirstOrDefaultAsync(c => c.Id == id);
            return item;
        }

        public async Task<List<ContactUs>> List()
        {
            var items = await db.ContactUs.Include(d => d.MessageReply).ToListAsync();
            return items;
        }
    }
}

