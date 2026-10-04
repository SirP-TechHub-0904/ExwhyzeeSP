using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExwhyzeeSP.Web.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class EventsControlController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Staff/EventsControl
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetmyEvents()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var events = db.Events.Where(x => x.UserId == userid || x.GeneralEvent == true).ToList();
            return Json(events);

        }

        [AllowAnonymous]
        public JsonResult LgaList(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return Json(new SelectList(new List<LocalGovs>(), "LGAName", "LGAName"));
            }
            var state = db.States.FirstOrDefault(x => x.StateName == Id);
            if (state == null)
            {
                return Json(new SelectList(new List<LocalGovs>(), "LGAName", "LGAName"));
            }
            var stateId = state.Id;
            var local = from s in db.LocalGovs
                        where s.StatesId == stateId
                        select s;

            return Json(new SelectList(local.ToArray(), "LGAName", "LGAName"));
        }

        //json to fetch subjects by class
        public JsonResult SubjectByClass(int Id)
        {
            if (Id <= 0)
            {
                return Json(new SelectList(new List<Subject>(), "Id", "SubjectName"));
            }
            var cl = db.ClassLevels.FirstOrDefault(x => x.Id == Id);
            if (cl == null)
            {
                return Json(new SelectList(new List<Subject>(), "Id", "SubjectName"));
            }
            var uId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var formTeacher = db.ClassLevels.Include(x=>x.User).FirstOrDefault(x => x.UserId == uId && x.Id == Id && x.ShowClass == true);
            var classid = cl.Id;
            var local = from s in db.Subjects
                        where s.ClassLevelId == classid
                        select s;

            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                local = local.Where(x=>x.ShowSubject == true).OrderBy(x => x.SubjectName);
            }
            else if (formTeacher != null)
            {
                local = local.Include(x=>x.User).Where(x => x.ClassLevelId == Id && x.ShowSubject == true).OrderBy(x => x.SubjectName);
            }
            else
            {
                local = local.Include(x => x.User).Where(x => x.UserId == uId && x.ShowSubject == true).OrderBy(x => x.SubjectName);
            }
            return Json(new SelectList(local.ToArray(), "Id", "SubjectName"));
        }

        //event
        #region
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddEventp(string subject, string description, DateTime start, DateTime end, string color, bool general, bool fday)
        {
            try
            {
                string u = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Event m = new Event();
                m.Subject = subject;
                m.DIscription = description;
                m.Start = start;
                m.End = end;
                m.Color = color;
                m.GeneralEvent = general;
                m.IsFullDay = fday;
                m.UserId = u;
                db.Events.Add(m);
                db.SaveChanges();
                TempData["success"] = "Event Added";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something Went Wrong. Try again.";
            }
            return RedirectToAction("Index", "Panel" ,new { area = "Staff" });
        }
        #endregion
    }
}

