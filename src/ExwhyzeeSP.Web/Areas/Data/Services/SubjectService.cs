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
    public class SubjectService : ISubjectService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SubjectService()
        {
        }

        public SubjectService(
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

        public async Task Create(Subject model, int id)
        {
            model.ClassLevelId = id;
            db.Subjects.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added a subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            
        }

        public async Task Delete(int? id)
        {
            var item = await db.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            var enrolsub = db.EnrolledSubjects.Where(x=>x.SubjectId == item.Id);
            foreach(var sub in enrolsub)
            {
                db.EnrolledSubjects.Remove(sub);
               // await db.SaveChangesAsync();
            }
            db.Subjects.Remove(item);
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
                tracker.Note = tracker.FullName + " " + "Deleted a subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          
        }

        public async Task Edit(Subject models)
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
                tracker.Note = tracker.FullName + " " + "Edited a subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task EnrolledSubject(EnrolledSubject model)
        {
            db.EnrolledSubjects.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added Enrolled Subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          
        }

        public async Task<Subject> Get(int? id)
        {
            var item = await db.Subjects.Include(x=>x.ClassLevel).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<IQueryable<SubjectListDto>> List(int? id)
        {

            IQueryable<Subject> item = from s in db.Subjects
                                        .Include(x => x.User).Include(x => x.ClassLevel).Where(x => x.ClassLevelId == id && x.ShowSubject == true && x.ClassLevel.ShowClass == true)
                                                      select s;
           // var item = db.Subjects.Include(x=>x.User).Where(x=>x.ClassLevelId == id);
            var output = item.Select(x => new SubjectListDto
            {
                SubjectName = x.SubjectName,
                ClassLevelId = id,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName,
                SubjectId = x.Id,
                UserId = x.UserId,
                ShowSubject = x.ShowSubject
            });
            return output.OrderBy(x=>x.SubjectName).AsQueryable();
        }

        public async Task<IQueryable<SubjectListDto>> AllList(int? id)
        {

            IQueryable<Subject> item = from s in db.Subjects
                                        .Include(x => x.User).Where(x => x.ClassLevelId == id)
                                       select s;
            // var item = db.Subjects.Include(x=>x.User).Where(x=>x.ClassLevelId == id);
            var output = item.Select(x => new SubjectListDto
            {
                SubjectName = x.SubjectName,
                ClassLevelId = id,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName,
                SubjectId = x.Id,
                UserId = x.UserId,
                ShowSubject = x.ShowSubject
            });
            return output.OrderBy(x => x.SubjectName).AsQueryable();
        }
    }
}

