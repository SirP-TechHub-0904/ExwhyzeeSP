using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using X.PagedList;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Owin.Security;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Web.Areas.Data.Services;

namespace ExwhyzeeSP.Web.Areas.SuperUser.Controllers
{

    [Authorize(Roles = "SuperAdmin")]
    [Area("SuperUser")]
    public class AdminUserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IUserManagerService _userService = new UserManagerService();

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public AdminUserController()
        {
        }

        public AdminUserController(UserManagerService userService, ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            _userService = userService;
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public async Task<ActionResult> UserList(string searchString, string currentFilter, int? page)
        {

            ViewBag.Roles = RoleManager.Roles.Where(x => x.Name != "SuperAdmin").ToList();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.AllUsers(searchString, currentFilter, page);
            ViewBag.countall = items.Count();
            //var items = await UserManager.Users.OrderBy(x => x.Surname).ToListAsync();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> DeleteAllUser(int id)
        {
            var classLevel = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == id);
            if (classLevel == null)
            {
                TempData["error"] = "Class not found.";
                return RedirectToAction("Index", "ClassLevels", new { area = "Admin" });
            }

            var studentCount = await db.Enrollments
                .Include(x => x.StudentProfile)
                .Where(x => x.ClassLevelId == id)
                .Select(x => x.StudentProfileId)
                .Distinct()
                .CountAsync();

            ViewBag.ClassLevel = classLevel;
            ViewBag.StudentCount = studentCount;
            ViewBag.ClassId = id;

            return View();
        }

        [HttpPost]
        [ActionName("DeleteAllUser")]
        public async Task<ActionResult> DeleteAllUserConfirmed(int id, string confirmCode = "")
        {
            if (string.IsNullOrWhiteSpace(confirmCode) || !confirmCode.Trim().Equals("CONFIRM", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "Mass deletion cancelled: You must type 'CONFIRM' to approve deleting all students in this class.";
                return RedirectToAction("DeleteAllUser", new { id = id });
            }

            var usk = await db.Enrollments.AsNoTracking().Include(x => x.User).Include(x => x.StudentProfile.user).Include(x => x.StudentProfile).Where(x => x.ClassLevelId == id).ToListAsync();
            int deletedCount = 0;
            foreach (var ioi in usk)
            {
                string userid = ioi.StudentProfile?.user?.Id;
                if (string.IsNullOrEmpty(userid)) continue;

                try
                {
                    var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userid);
                    var studentp = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.user.Id == user.Id);
                    if (studentp == null) continue;

                    var enro = await db.Enrollments.Include(x => x.StudentProfile).Where(x => x.StudentProfileId == studentp.Id).ToListAsync();
                    foreach (var enro1 in enro)
                    {
                        var enroSub = await db.EnrolledSubjects.Where(x => x.EnrollmentId == enro1.Id).ToListAsync();
                        foreach (var sub in enroSub)
                        {
                            db.EnrolledSubjects.Remove(sub);
                        }

                        var ate1 = await db.AttendanceDetails.Where(x => x.EnrollmentId == enro1.Id).ToListAsync();
                        foreach (var i in ate1)
                        {
                            db.AttendanceDetails.Remove(i);
                        }

                        var ate = await db.Attendances.Where(x => x.EnrollmentId == enro1.Id).ToListAsync();
                        foreach (var i in ate)
                        {
                            db.Attendances.Remove(i);
                        }

                        var atee = await db.AssignmentAnswers.Where(x => x.EnrollementId == enro1.Id).ToListAsync();
                        foreach (var i in atee)
                        {
                            db.AssignmentAnswers.Remove(i);
                        }

                        db.Enrollments.Remove(enro1);
                    }
                    await db.SaveChangesAsync();

                    var evnt = await db.Events.Where(x => x.UserId == user.Id).ToListAsync();
                    foreach (var i in evnt)
                    {
                        db.Events.Remove(i);
                    }

                    var df = await db.Defaulters.Where(x => x.ProfileId == studentp.Id).ToListAsync();
                    foreach (var i in df)
                    {
                        db.Defaulters.Remove(i);
                    }

                    if (studentp.ImageId != null)
                    {
                        var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == studentp.ImageId);
                        if (img != null)
                        {
                            db.ImageModel.Remove(img);
                        }
                    }

                    await UserManager.RemoveFromRoleAsync(userid, "Student");
                    db.StudentProfiles.Remove(studentp);
                    await db.SaveChangesAsync();

                    if (user != null)
                    {
                        db.Users.Remove(user);
                        await db.SaveChangesAsync();
                        deletedCount++;
                    }
                }
                catch (Exception)
                {
                }
            }

            TempData["success"] = $"Successfully deleted {deletedCount} student accounts and their associated records.";
            return RedirectToAction("Details", "ClassLevels", new { id = id, area = "Admin" });
        }
        public async Task<ActionResult> DeleteUser(string userid)
        {
            try
            {


                var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userid);

                var studentp = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.user.Id == user.Id);
                var staffp = await db.StaffProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.user.Id == user.Id);

                if (studentp != null)
                {
                    var enro = await db.Enrollments.Include(x => x.StudentProfile).Where(x => x.StudentProfileId == studentp.Id).ToListAsync();
                    var enro1 = await db.Enrollments.Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.StudentProfileId == studentp.Id);
                    if (enro1 != null)
                    {
                        var enroSub = await db.EnrolledSubjects.Where(x => x.EnrollmentId == enro1.Id).ToListAsync();
                    }

                    var evnt = await db.Events.Where(x => x.UserId == user.Id).ToListAsync();
                    if (evnt.Count() > 0)
                    {
                        foreach (var i in evnt)
                        {
                            db.Events.Remove(i);
                            await db.SaveChangesAsync();
                        }

                    }

                    var df = await db.Defaulters.Where(x => x.ProfileId == studentp.Id).ToListAsync();
                    if (df.Count() > 0)
                    {
                        foreach (var i in df)
                        {
                            db.Defaulters.Remove(i);
                            await db.SaveChangesAsync();
                        }

                    }
                    if (enro1 != null)
                    {
                        var ate1 = await db.AttendanceDetails.Where(x => x.EnrollmentId == enro1.Id).ToListAsync();
                        if (ate1.Count() > 0)
                        {
                            foreach (var i in ate1)
                            {
                                db.AttendanceDetails.Remove(i);
                                await db.SaveChangesAsync();
                            }

                        }
                        var ate = await db.Attendances.Where(x => x.EnrollmentId == enro1.Id).ToListAsync();
                        if (ate.Count() > 0)
                        {
                            foreach (var i in ate)
                            {
                                db.Attendances.Remove(i);
                                await db.SaveChangesAsync();
                            }

                        }
                        var atee = await db.AssignmentAnswers.Where(x => x.EnrollementId == enro1.Id).ToListAsync();
                        if (atee.Count() > 0)
                        {
                            foreach (var i in atee)
                            {
                                db.AssignmentAnswers.Remove(i);
                                await db.SaveChangesAsync();
                            }

                        }
                    }

                    var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == studentp.ImageId);
                    if (img != null)
                    {
                        db.ImageModel.Remove(img);
                        await db.SaveChangesAsync();
                    }

                    await UserManager.RemoveFromRoleAsync(userid, "Student");

                    db.StudentProfiles.Remove(studentp);
                    await db.SaveChangesAsync();

                    if (user != null)
                    {
                        db.Users.Remove(user);
                        await db.SaveChangesAsync();
                        TempData["success"] = "Deleted";
                    }
                    else
                    {

                    }
                }
                else if (staffp != null)
                {

                    var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == staffp.ImageId);
                    if (img != null)
                    {
                        db.ImageModel.Remove(img);
                        await db.SaveChangesAsync();
                    }

                    await UserManager.RemoveFromRoleAsync(userid, "Staff");

                    db.StaffProfiles.Remove(staffp);
                    await db.SaveChangesAsync();

                    if (user != null)
                    {
                        db.Users.Remove(user);
                        await db.SaveChangesAsync();
                        TempData["success"] = "Deleted";
                    }
                    else
                    {

                    }
                }
               
                return RedirectToAction("UserList");
            }
            catch (Exception e)
            {
                TempData["error"] = "Not Successful";
                return RedirectToAction("UserList");
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult CreateAccount()
        {
            return View();
        }

        public async Task<ActionResult> ReadOnly()
        {
            RegisterViewModel readonlymodel = new RegisterViewModel();
            readonlymodel.Username = "Education Sec";
            readonlymodel.Email = "dmmm@admin.com";
            var userreadonlymodel = new ApplicationUser { UserName = readonlymodel.Username, Email = readonlymodel.Email, DateRegistered = DateTime.UtcNow.AddHours(1) };
            readonlymodel.Password = "dmmm@motherofchrist";

            var resultreadonlymodel = await UserManager.CreateAsync(userreadonlymodel, readonlymodel.Password);
            if (resultreadonlymodel.Succeeded)
            {

                var role = new IdentityRole("Read Only");
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First().Description);
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                await UserManager.AddToRoleAsync(userreadonlymodel.Id, "Read Only");
                await UserManager.AddToRoleAsync(userreadonlymodel.Id, "Admin");
                ////
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAccount(RegisterViewModel model)
        {
            model.Username = "SuperAdmin";
            model.Email = "superadmin@super.com";
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email, DateRegistered = DateTime.UtcNow.AddHours(1) };
            model.Password = "super@123";

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var role = new IdentityRole("SuperAdmin");
                var role1 = new IdentityRole("Staff");
                var role2 = new IdentityRole("Student");
                var role3 = new IdentityRole("Admin");
                var role4 = new IdentityRole("FormTeacher");
                var role5 = new IdentityRole("Read Only");
                var role6 = new IdentityRole("Finance");
                var role7 = new IdentityRole("Developer");
              
                await RoleManager.CreateAsync(role);
                await RoleManager.CreateAsync(role1);
                await RoleManager.CreateAsync(role2);
                await RoleManager.CreateAsync(role3);
                await RoleManager.CreateAsync(role4);
                await RoleManager.CreateAsync(role5);
                await RoleManager.CreateAsync(role6);
                await RoleManager.CreateAsync(role7);

                await UserManager.AddToRoleAsync(user.Id, "SuperAdmin");
                ////
                ///

                ///
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return RedirectToAction("CreateAdmin");
            }
            TempData["error"] = "Contact Administrator";
            return View();

        }


        public async Task<ActionResult> CreateAdmin(RegisterViewModel model)
        {
            RegisterViewModel readonlymodel = new RegisterViewModel();
            readonlymodel.Username = "Dmmm";
            readonlymodel.Email = "dmmm@admin.com";
            var userreadonlymodel = new ApplicationUser { UserName = readonlymodel.Username, Email = readonlymodel.Email, DateRegistered = DateTime.UtcNow.AddHours(1) };
            readonlymodel.Password = "dmmm@motherofchrist";

            var resultreadonlymodel = await UserManager.CreateAsync(userreadonlymodel, readonlymodel.Password);
            if (resultreadonlymodel.Succeeded)
            {

                await UserManager.AddToRoleAsync(userreadonlymodel.Id, "Read Only");
                ////

            }
            model.Username = "Admin";
            model.Email = "admin@admin.com";
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email, DateRegistered = DateTime.UtcNow.AddHours(1) };
            model.Password = "Admin@123";

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {

                await UserManager.AddToRoleAsync(user.Id, "Admin");
                ////
                ///

                return RedirectToAction("Create", "Settings", new { area = "Admin" });
            }
            return View();

        }


        #region pin upload



        #endregion


    }
}

