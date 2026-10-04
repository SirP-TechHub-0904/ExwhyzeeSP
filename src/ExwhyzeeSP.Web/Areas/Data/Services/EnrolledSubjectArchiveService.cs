using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Core.Entities.ResultArchive;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
// // // using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class EnrolledSubjectArchiveService : IEnrolledSubjectArchiveService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EnrolledSubjectArchiveService()
        {
        }

        public EnrolledSubjectArchiveService(
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

        public async Task Create(EnrolledSubjectArchive model)
        {
            db.EnrolledSubjectArchive.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added Enrolled Subject Archive";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            
        }

        public async Task Delete(int? id)
        {
            var item = await db.EnrolledSubjectArchive.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                db.EnrolledSubjectArchive.Remove(item);
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
                    tracker.Note = tracker.FullName + " " + "Deleted Enrolled Subject Archive";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
                
            }
        }

        //public async Task Edit(EnrolledSubject models)
        //{
            
        //    db.Entry(models).State = EntityState.Modified;
        //    await db.SaveChangesAsync();
        //}

        public async Task Edit(EnrolledSubjectArchive models)
        {
            try
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
                    tracker.Note = tracker.FullName + " " + "Edited Enrolled Subject Archive";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }
            catch (Exception e)
            {
                try
                {
                    var DFD = await db.EnrolledSubjectArchive.FindAsync(models.Id);
                    DFD.ExamScore = models.ExamScore;
                    DFD.TestScore = models.TestScore;
                    DFD.TotalScore = models.TotalScore;
                    DFD.GradingOption = models.GradingOption;
                    DFD.IsOffered = models.IsOffered;
                    db.Entry(DFD).State = EntityState.Modified;
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
                        tracker.Note = tracker.FullName + " " + "Edited Enrolled Subject Archive";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                   
                }
                catch (Exception es)
                {

                }


            }

        }
        public async Task EditScore(int id)
        {
            var models = await db.EnrolledSubjectArchive.FirstOrDefaultAsync(x => x.Id == id);

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
                tracker.Note = tracker.FullName + " " + "Added Enrolled Subject Archive Score";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            
        }

        public async Task<EnrolledSubjectArchive> Get(int? id)
        {
            var item = await db.EnrolledSubjectArchive.Include(x=>x.Enrollments).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<EnrolledSubjectArchive>> List()
        {
            var item = await db.EnrolledSubjectArchive.ToListAsync();
            return item;
        }
    }
}

