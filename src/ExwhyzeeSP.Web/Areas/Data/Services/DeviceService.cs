using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// // // using Microsoft.AspNetCore.Http;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class DeviceService : IDeviceService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DeviceService()
        {
        }

        public DeviceService(
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

        public async Task Create(ApprovedDevice model)
        {
            var uId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = uId;
            model.Date = DateTime.UtcNow.AddHours(1);
            db.ApprovedDevices.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added a approve device for logging in";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Delete(int? id)
        {
            var device = await db.ApprovedDevices.FirstOrDefaultAsync(x => x.Id == id);
            if(device != null)
            {
                db.ApprovedDevices.Remove(device);
                await db.SaveChangesAsync();


                //Add Tracking
                var userId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user?.UserName ?? "";
                    tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "deleted a approve device for logging in";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }
          
        }

        public async Task<List<ApprovedDevice>> DeviceList()
        {
            var device = await db.ApprovedDevices.Include(x=>x.User).ToListAsync();
            return device;
        }

        public async Task Edit(ApprovedDevice model)
        {
            db.Entry(model).State = EntityState.Modified;
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user?.UserName ?? "";
                tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "edited an approve device for logging in";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<ApprovedDevice> Get(int? id)
        {
            var device = await db.ApprovedDevices.Include(x=>x.User).FirstOrDefaultAsync(x => x.Id == id);
            return device;
        }
    }
}

