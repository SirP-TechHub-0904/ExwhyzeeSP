using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// // // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class IncomeService : IIncomeService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

                private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IncomeService()
        {
        }

        public IncomeService(
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

        public async Task Create(Income item)
        {
            var uId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
            item.Date = DateTime.UtcNow.AddHours(1);
            item.UserId = uId;
            db.Incomes.Add(item);
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
                tracker.Note = tracker.FullName + " " + "Added an income source";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Delete(int? id)
        {
            var item = await db.Incomes.FirstOrDefaultAsync(x => x.Id == id);
            db.Incomes.Remove(item);
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
                tracker.Note = tracker.FullName + " " + "Deleted an income source";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Edit(Income item)
        {
            db.Entry(item).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "Edited an income source";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          
        }

        public async Task<Income> Get(int? id)
        {
            var item = await db.Incomes.FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<Income>> GetUserIncome(string userId)
        {
            var item = await db.Incomes.Where(x => x.UserId == userId).ToListAsync();
            return item;
        }

        public async Task<List<Income>> IncomeList()
        {
            var item = await db.Incomes.ToListAsync();
            return item;
        }

        //public async Task<List<Income>> PaymentAmountIncomeList()
        //{
        //    var uId = HttpHelper.Current.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefault(x => x.StudentProfile.UserId == uId);
        //    var pay = await db.PaymentAmounts.Include(x => x.ClassLevel).Include(x => x.Income).Where(x => x.ClassLevelId == enrol.ClassLevelId).ToListAsync();
        //    foreach(var incom in pay)
        //    {
        //        var item = db.Incomes.Where(x=>x.Id == incom.Id).ToList();
        //        return item;
        //    }
        //    return pay;
        //}
    }
}

