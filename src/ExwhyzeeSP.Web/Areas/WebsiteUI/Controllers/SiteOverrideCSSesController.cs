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
using ExwhyzeeSP.Core.Entities.UI;

namespace ExwhyzeeSP.Web.Areas.WebsiteUI.Controllers
{
    [Area("WebsiteUI")]
    public class SiteOverrideCSSesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteOverrideCSSes
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteOverrideCSSs.ToListAsync());
        }

        // GET: WebsiteUI/SiteOverrideCSSes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteOverrideCSS siteOverrideCSS = await db.SiteOverrideCSSs.FindAsync(id);
            if (siteOverrideCSS == null)
            {
                return NotFound();
            }
            return View(siteOverrideCSS);
        }

        // GET: WebsiteUI/SiteOverrideCSSes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteOverrideCSSes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Content,Show")] SiteOverrideCSS siteOverrideCSS)
        {
            if (ModelState.IsValid)
            {
                db.SiteOverrideCSSs.Add(siteOverrideCSS);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteOverrideCSS);
        }

        // GET: WebsiteUI/SiteOverrideCSSes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteOverrideCSS siteOverrideCSS = await db.SiteOverrideCSSs.FindAsync(id);
            if (siteOverrideCSS == null)
            {
                return NotFound();
            }
            return View(siteOverrideCSS);
        }

        // POST: WebsiteUI/SiteOverrideCSSes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Content,Show")] SiteOverrideCSS siteOverrideCSS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteOverrideCSS).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteOverrideCSS);
        }

        // GET: WebsiteUI/SiteOverrideCSSes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteOverrideCSS siteOverrideCSS = await db.SiteOverrideCSSs.FindAsync(id);
            if (siteOverrideCSS == null)
            {
                return NotFound();
            }
            return View(siteOverrideCSS);
        }

        // POST: WebsiteUI/SiteOverrideCSSes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteOverrideCSS siteOverrideCSS = await db.SiteOverrideCSSs.FindAsync(id);
            db.SiteOverrideCSSs.Remove(siteOverrideCSS);
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


