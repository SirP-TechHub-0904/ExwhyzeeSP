using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExwhyzeeSP.Web.Areas.SuperUser.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("SuperUser")]
    public class TrackerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: SuperUser/Tracker
        public async Task<ActionResult> Index()
        {
            var tracker = await db.Trackers.OrderByDescending(x => x.Id).ToListAsync();
            return View(tracker);
        }

        // GET: SuperUser/Tracker/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SuperUser/Tracker/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuperUser/Tracker/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SuperUser/Tracker/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SuperUser/Tracker/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SuperUser/Tracker/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SuperUser/Tracker/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}


