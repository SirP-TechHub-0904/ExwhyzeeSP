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
    public class SiteContactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteContacts
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteContacts.ToListAsync());
        }

        // GET: WebsiteUI/SiteContacts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteContact siteContact = await db.SiteContacts.FindAsync(id);
            if (siteContact == null)
            {
                return NotFound();
            }
            //header
            var header = await db.SiteHeaders.FirstOrDefaultAsync();
            if (header != null)
            {
                ViewBag.head = header.Content;
            }

            var overridecss = await db.SiteOverrideCSSs.FirstOrDefaultAsync();
            if (overridecss != null)
            {
                ViewBag.overridecss = overridecss.Content;
            }
            //header
            var footerJs = await db.SiteFooterJSs.FirstOrDefaultAsync();
            if (footerJs != null)
            {
                ViewBag.footerJs = footerJs.Content;
            }

            return View(siteContact);
        }

        // GET: WebsiteUI/SiteContacts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteContacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Content,Show")] SiteContact siteContact)
        {
            if (ModelState.IsValid)
            {
                db.SiteContacts.Add(siteContact);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteContact);
        }

        // GET: WebsiteUI/SiteContacts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteContact siteContact = await db.SiteContacts.FindAsync(id);
            if (siteContact == null)
            {
                return NotFound();
            }
            return View(siteContact);
        }

        // POST: WebsiteUI/SiteContacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Content,Show")] SiteContact siteContact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteContact).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteContact);
        }

        // GET: WebsiteUI/SiteContacts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteContact siteContact = await db.SiteContacts.FindAsync(id);
            if (siteContact == null)
            {
                return NotFound();
            }
            return View(siteContact);
        }

        // POST: WebsiteUI/SiteContacts/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteContact siteContact = await db.SiteContacts.FindAsync(id);
            db.SiteContacts.Remove(siteContact);
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


