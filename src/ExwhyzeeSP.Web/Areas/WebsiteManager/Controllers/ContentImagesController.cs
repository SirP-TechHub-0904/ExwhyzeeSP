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
using System.IO;

namespace ExwhyzeeSP.Web.Areas.WebsiteManager.Controllers
{
    [Area("WebsiteManager")]
    public class ContentImagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteManager/ContentImages
        public async Task<ActionResult> Index()
        {
            return View(await db.ContentImages.ToListAsync());
        }

        // GET: WebsiteManager/ContentImages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentImage contentImage = await db.ContentImages.FindAsync(id);
            if (contentImage == null)
            {
                return NotFound();
            }
            return View(contentImage);
        }

        // GET: WebsiteManager/ContentImages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteManager/ContentImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ContentImage contentImage, List<IFormFile> upload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (upload.Count() > 0)
                    {
                        foreach (var image in upload)
                        {

                            if (image != null && image.Length > 0)
                            {


                                string date1 = DateTime.UtcNow.AddHours(1).ToString("ddMMyyyyhhmm");
                                string name = date1 + "-" + image.FileName;
                                string fileName = Path.GetFileName(name);
                                contentImage.ImageUrl = fileName;
                                contentImage.FIleName = fileName;
                                fileName = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + ("~/Aq_Image/"), fileName);
                                image.SaveAs(fileName);

                                db.ContentImages.Add(contentImage);
                                await db.SaveChangesAsync();
                            }
                        }
                        return RedirectToAction("Index");
                    }
                }catch(Exception c)
                {
                    TempData["error"] = c;
                }
            }

            return View(contentImage);
        }

        // GET: WebsiteManager/ContentImages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentImage contentImage = await db.ContentImages.FindAsync(id);
            if (contentImage == null)
            {
                return NotFound();
            }
            return View(contentImage);
        }

        // POST: WebsiteManager/ContentImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,ImageUrl,FIleName")] ContentImage contentImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentImage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(contentImage);
        }

        // GET: WebsiteManager/ContentImages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentImage contentImage = await db.ContentImages.FindAsync(id);
            if (contentImage == null)
            {
                return NotFound();
            }
            return View(contentImage);
        }

        // POST: WebsiteManager/ContentImages/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ContentImage contentImage = await db.ContentImages.FindAsync(id);
            //string[] files = Directory.GetFiles("~/Aq_Image/");
            if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + ("~/Aq_Image/" + contentImage.ImageUrl)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + ("~/Aq_Image/" + contentImage.ImageUrl));
            }
            db.ContentImages.Remove(contentImage);
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


