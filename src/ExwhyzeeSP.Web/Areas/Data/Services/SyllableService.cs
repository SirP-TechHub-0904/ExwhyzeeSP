using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// // // using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class SyllableService : ISyllableService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SyllableService()
        {
        }

        public SyllableService(
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

        public async Task Create(Syllable model)
        {
           
            db.syllables.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added a Syllable";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          
        }

        public async Task Delete(int? id)
        {
            var item = await db.syllables.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                db.syllables.Remove(item);
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
                    tracker.Note = tracker.FullName + " " + "Deleted a Syllable";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }
        }

        public async Task Edit(Syllable models)
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
                tracker.Note = tracker.FullName + " " + "Edited a Syllable";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          
        }

        public async Task<Syllable> Get(int? id)
        {
            var item = await db.syllables.Include(x => x.Session).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<Syllable>> ListAll()
        {
            var item = await db.syllables.Include(x=>x.Session).ToListAsync();
            return item;
        }

        public async Task<List<Syllable>> ListByType(string title)
        {
            var item = await db.syllables.Include(x => x.Session).Where(x=>x.Title.Contains(title)).ToListAsync();
            return item;
        }
    }
}

