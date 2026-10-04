using ExwhyzeeSP.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
// // // using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Entities;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{

    public class SessionService : ISessionService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SessionService()
        {
        }

        public SessionService(
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



        public async Task Create(Session model)
        {
            var existingSession = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == model.SessionYear);
            var cSession = db.Sessions.Count();
            if (existingSession == null)
            {
                string[] terms = new string[] { "First", "Second", "Third" };
                for (int i = 0; i <= terms.Length - 1; i++)
                {
                    var newSession = new Session
                    {
                        SessionYear = model.SessionYear,
                        SchoolPrincipal = model.SchoolPrincipal ?? "Principal",
                        Term = terms[i],
                        Status = (cSession == 0 && terms[i] == "First") ? SessionStatus.Current : SessionStatus.NotUsed
                    };

                    db.Sessions.Add(newSession);
                    await db.SaveChangesAsync();
                }

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
                    tracker.Note = tracker.FullName + " " + "Added a session";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
              
            }
            //else
            //{
            //    return null;
            //}

        }

        public async Task Delete(int? id)
        {
            var term = await db.Sessions.FirstOrDefaultAsync(x => x.Id == id);

            var sessionNames = db.Sessions.Where(x => x.SessionYear == term.SessionYear);

            foreach (var item in sessionNames)
            {
                db.Sessions.Remove(item);

            }
            db.SaveChanges();

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
                tracker.Note = tracker.FullName + " " + "Deleted a session";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Edit(Session models)
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
                tracker.Note = tracker.FullName + " " + "Edited a session";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<Session> Get(int? id)
        {
            var item = db.Sessions.FirstOrDefaultAsync(x => x.Id == id);
            return await item;
        }

        public async Task<List<SchoolSessionDto>> GetAllSession()
        {
            var item = db.Sessions.OrderByDescending(x => x.SessionYear).ThenBy(x => x.Term);
            var output = item.Select(x => new SchoolSessionDto
            {
                FullSession = x.SessionYear + " - " + x.Term + " Term",
                Id = x.Id,
                SessionStatus = x.Status,
                Year = x.SessionYear
            });
            return await output.ToListAsync();
        }

        public async Task<int> GetCurrentSessionId()
        {
            var session = db.Sessions.OrderByDescending(x => x.Id);
            var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            return currentSession.Id;
        }


        public async Task<string> GetCurrentSession()
        {
            var session = db.Sessions.OrderByDescending(x => x.Id);
            var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            return currentSession.SessionYear;
        }

        public async Task<string> GetCurrentSessionTerm()
        {
            var session = db.Sessions.OrderByDescending(x => x.Id);
            var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            return currentSession.Term;
        }

        public async Task<SchoolSessionDto> GetInfo(int id)
        {
            var item = db.Sessions.Where(x => x.Id == id);
            var output = item.Select(x => new SchoolSessionDto
            {
                FullSession = x.SessionYear + " - " + x.Term + " Term",
                Id = x.Id
            });
            return await output.FirstOrDefaultAsync();
        }

        public async Task<List<Session>> List()
        {
            var list = db.Sessions.OrderBy(x => x.Id);
            return await list.ToListAsync();
        }

        public async Task<bool> NextSession()
        {
            try
            {
                var session = db.Sessions.OrderByDescending(x => x.Id);
                if (session != null)
                {

                    var currentSession = session.Where(x => x.Status == SessionStatus.Current).Single();
                    var next = db.Sessions.Where(x => x.Id > currentSession.Id).Take(1);
                    //var nxt = db.Sessions
                    //var nxt = (from x in session where x.Id < currentSession.Id orderby x.Id descending select x).FirstOrDefault();

                    var nextsession = await db.Sessions.FirstOrDefaultAsync(x => x.Id == next.FirstOrDefault().Id);
                    nextsession.Status = SessionStatus.Current;

                    db.Entry(nextsession).State = EntityState.Modified;

                    var oldsession = await db.Sessions.FirstOrDefaultAsync(x => x.Id == currentSession.Id);
                    oldsession.Status = SessionStatus.Used;
                    db.Entry(oldsession).State = EntityState.Modified;
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
                        tracker.Note = tracker.FullName + " " + "Moved a session to the next";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                    

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        public async Task<bool> PreviousSession()
        {
            try
            {


                var session = db.Sessions.OrderByDescending(x => x.Id);
                if (session != null)
                {
                    var currentSession = session.Where(x => x.Status == SessionStatus.Current).Single();
                    var prev = db.Sessions.Where(x => x.Id < currentSession.Id).OrderByDescending(x => x.Id).Take(1);
                    //var nxt = db.Sessions
                    //var nxt = (from x in session where x.Id < currentSession.Id orderby x.Id descending select x).FirstOrDefault();

                    var prevsession = await db.Sessions.FirstOrDefaultAsync(x => x.Id == prev.FirstOrDefault().Id);
                    prevsession.Status = SessionStatus.Current;


                    db.Entry(prevsession).State = EntityState.Modified;

                    var oldsession = await db.Sessions.FirstOrDefaultAsync(x => x.Id == currentSession.Id);
                    oldsession.Status = SessionStatus.Used;
                    db.Entry(oldsession).State = EntityState.Modified;
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
                        tracker.Note = tracker.FullName + " " + "Moved a session to the previous";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                 

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}

