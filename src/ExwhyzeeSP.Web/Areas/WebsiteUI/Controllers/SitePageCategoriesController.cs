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
    public class SitePageCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SitePageCategories
        public async Task<ActionResult> Index()
        {
            return View(await db.SitePageCategories.ToListAsync());
        }

        // GET: WebsiteUI/SitePageCategories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePageCategory sitePageCategory = await db.SitePageCategories.FindAsync(id);
            if (sitePageCategory == null)
            {
                return NotFound();
            }
            return View(sitePageCategory);
        }

        // GET: WebsiteUI/SitePageCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SitePageCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Title,Show")] SitePageCategory sitePageCategory)
        {
            if (ModelState.IsValid)
            {
                db.SitePageCategories.Add(sitePageCategory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(sitePageCategory);
        }

        // GET: WebsiteUI/SitePageCategories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePageCategory sitePageCategory = await db.SitePageCategories.FindAsync(id);
            if (sitePageCategory == null)
            {
                return NotFound();
            }
            return View(sitePageCategory);
        }

        // POST: WebsiteUI/SitePageCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Title,Show")] SitePageCategory sitePageCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sitePageCategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(sitePageCategory);
        }

        // GET: WebsiteUI/SitePageCategories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePageCategory sitePageCategory = await db.SitePageCategories.FindAsync(id);
            if (sitePageCategory == null)
            {
                return NotFound();
            }
            return View(sitePageCategory);
        }

        // POST: WebsiteUI/SitePageCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SitePageCategory sitePageCategory = await db.SitePageCategories.FindAsync(id);
            db.SitePageCategories.Remove(sitePageCategory);
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


