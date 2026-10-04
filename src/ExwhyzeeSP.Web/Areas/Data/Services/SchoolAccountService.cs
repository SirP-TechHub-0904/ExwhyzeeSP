using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
// // // using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class SchoolAccountService : ISchoolAccountService

    {
        private ApplicationDbContext db = new ApplicationDbContext();

                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SchoolAccountService()
        {
        }

        public SchoolAccountService(
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


        public async Task Create(SchoolAccount model)
        {
            db.SchoolAccounts.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added School Account";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Delete(int? id)
        {
            var acct = await db.SchoolAccounts.FirstOrDefaultAsync(x => x.Id == id);
            if (acct != null)
            {
                db.SchoolAccounts.Remove(acct);
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
                    tracker.Note = tracker.FullName + " " + "Deleted School Account";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }
        }

        public async Task Edit(SchoolAccount models)
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
                tracker.Note = tracker.FullName + " " + "Edited School Account";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<SchoolAccount> Get(int? id)
        {
            var acct = await db.SchoolAccounts.FirstOrDefaultAsync(x => x.Id == id);
            return acct;
        }

        public async Task<List<SchoolAccount>> List()
        {
            var acct = await db.SchoolAccounts.OrderBy(x => x.AcctName).ToListAsync();
            return acct;
        }
    }
}

