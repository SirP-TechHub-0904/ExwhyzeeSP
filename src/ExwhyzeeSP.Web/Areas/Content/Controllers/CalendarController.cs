using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExwhyzeeSP.Web.Areas.Content.Controllers
{
    [Area("Content")]
    public class CalendarController : Controller
    {
        // GET: Content/Calendar
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetEvents()
        {
            using (ApplicationDbContext dc = new ApplicationDbContext())
            {
                var events = dc.Events.ToList();
                return Json(events);
            }
        }
    }
}

