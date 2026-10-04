using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;

namespace ExwhyzeeSP.Web.Areas.Content.Controllers
{
    [Authorize]
    [Area("Content")]
    public class HelpsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Content/Helps
        public async Task<ActionResult> Index()
        {
            return View(await db.Helps.ToListAsync());
        }

        // GET: Content/Helps/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Help help = await db.Helps.FindAsync(id);
            if (help == null)
            {
                return NotFound();
            }
            return View(help);
        }

        // GET: Content/Helps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Content/Helps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Title,HelpUrl,Type")] Help help)
        {
            if (ModelState.IsValid)
            {
                db.Helps.Add(help);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(help);
        }

        // GET: Content/Helps/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Help help = await db.Helps.FindAsync(id);
            if (help == null)
            {
                return NotFound();
            }
            return View(help);
        }

        // POST: Content/Helps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Title,HelpUrl,Type")] Help help)
        {
            if (ModelState.IsValid)
            {
                db.Entry(help).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(help);
        }

        // GET: Content/Helps/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Help help = await db.Helps.FindAsync(id);
            if (help == null)
            {
                return NotFound();
            }
            return View(help);
        }

        // POST: Content/Helps/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Help help = await db.Helps.FindAsync(id);
            db.Helps.Remove(help);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}


