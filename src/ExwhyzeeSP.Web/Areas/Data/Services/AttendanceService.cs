using ExwhyzeeSP.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
// // // using Microsoft.AspNetCore.Http;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class AttendanceService : IAttendanceService
    {
        private ApplicationDbContext db = new ApplicationDbContext();


                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AttendanceService()
        {
        }

        public AttendanceService(
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


        public async Task AddAllStudentsInClass(int id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var enrolledStudents = db.Enrollments.Include(x => x.User).Include(e => e.StudentProfile).Include(x => x.Session).Where(s => s.ClassLevelId == id && s.Session.Status == SessionStatus.Current);

            foreach (var it in enrolledStudents)
            {
                AttendanceDetail attendanceDetail = new AttendanceDetail();
                attendanceDetail.StudentId = it.StudentProfileId;
                attendanceDetail.UserId = it.StudentProfile.UserId;
                db.AttendanceDetails.Add(attendanceDetail);
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
                    tracker.Note = tracker.FullName + " " + "Added an attendance";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }

        }

        public async Task Create(Attendance model)
        {
            db.Attendances.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added an attendance";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task Edit(Attendance model)
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
                tracker.Note = tracker.FullName + " " + "Edited attendance";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<Attendance>> ListAttendanceByClassBySession(int id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var attend = db.Attendances.Include(x => x.AttendanceDetails).Where(x => x.ClassLevelId == id && x.SessionId == currentSession.Id);
            return await attend.ToListAsync();

        }

        public async Task<List<AttendanceDetail>> ListAttendanceDetail(int id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var attend = db.AttendanceDetails.Include(x => x.StudentProfile).Include(c => c.User).Where(x => x.SessionId == currentSession.Id && x.AttendanceId == id);
            return await attend.ToListAsync();
        }

        public async Task<List<AttendanceDetail>> ListAttendanceDetailByStudent(int id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var attend = db.AttendanceDetails.Include(x => x.StudentProfile).Include(c => c.User).Where(x => x.SessionId == currentSession.Id && x.StudentId == id);
            return await attend.ToListAsync();
        }

        public async Task UpdateAttendance(AttendanceDetail model)
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
                tracker.Note = tracker.FullName + " " + "Updated an attendance";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }
        //public async Task AttendanceAllStudent(Attendance model)
        //{
        //    db.Attendances.Add(model);
        //    await db.SaveChangesAsync();
        //}


        //public async Task Edit(Attendance models)
        //{
        //    db.Entry(models).State = EntityState.Modified;
        //    await db.SaveChangesAsync();
        //}


    }
}

