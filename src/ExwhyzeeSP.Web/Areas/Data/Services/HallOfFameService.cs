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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class HallOfFameService : IHallOfFameService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HallOfFameService()
        {
        }

        public HallOfFameService(
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

        public async Task Create(HallOfFame model, IFormFile upload)
        {
            
            if (upload != null && upload.Length > 0)
            {


                // Find its length and convert it to byte array
                int ContentLength = (int)upload.Length;

                // Create Byte Array
                byte[] bytImg = new byte[ContentLength];

                // Read Uploaded file in Byte Array
                upload.OpenReadStream().Read(bytImg, 0, ContentLength);

                model.Image = bytImg;
                

            }
            model.DateCreated = DateTime.UtcNow.AddHours(1);
            db.HallOfFames.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added Hall Of Fame";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

        }

        public async Task Delete(int? id)
        {
            var fame = await db.HallOfFames.FirstOrDefaultAsync(x => x.Id == id);
            if (fame != null)
            {
                db.HallOfFames.Remove(fame);
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
                    tracker.Note = tracker.FullName + " " + "Deleted Hall Of Fame";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }

        }

        public async Task Edit(HallOfFame models, IFormFile upload)
        {

                if (upload != null && upload.Length > 0)
                {


                    // Find its length and convert it to byte array
                    int ContentLength = (int)upload.Length;

                    // Create Byte Array
                    byte[] bytImg = new byte[ContentLength];

                    // Read Uploaded file in Byte Array
                    upload.OpenReadStream().Read(bytImg, 0, ContentLength);

                    models.Image = bytImg;
                   
                }
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
                tracker.Note = tracker.FullName + " " + "Edited Hall Of Fame";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

        }

        public async Task<HallOfFame> Get(int? id)
        {
            var fame = await db.HallOfFames.FirstOrDefaultAsync(x => x.Id == id);
            return fame;
        }

        public async Task<List<HallOfFame>> List()
        {
            var fame = await db.HallOfFames.OrderBy(x=>x.SortOrder).ToListAsync();
            return fame;
        }
    }
}

