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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Web.Areas.Data.Services;

namespace ExwhyzeeSP.Web.Areas.Content.Controllers
{
    [Area("Content")]
    public class ContactUsChatController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IContactUsService _contactusService = new ContactUsService();


        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public ContactUsChatController()
        {

        }
        public ContactUsChatController(
            ContactUsService contactuservice
            )
        {
            _contactusService = contactuservice;
        }
        // GET: Content/ContactUsChat
        public async Task<ActionResult> Index()
        {
            var items = await _contactusService.List();
            return View(items);
        }

        // GET: Content/ContactUsChat/Details/5
        public async Task<ActionResult> Chat(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _contactusService.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // GET: Content/ContactUsChat/Create
        public ActionResult NewChat()
        {
            return View();
        }

        // POST: Content/ContactUsChat/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewChat(ContactUs contactUs)
        {
            if (ModelState.IsValid)
            {
                await _contactusService.Create(contactUs);
                return RedirectToAction("Chat");
            }

            return View(contactUs);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ChatReply(string message, int id)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                MessageReply msg = new MessageReply();
                msg.userId = user;
                msg.ReplyMessage = message;
                await _contactusService.Reply(msg, id);
                return RedirectToAction("Chat", new { id = id });
            }
            catch
            {

            }
            TempData["error"] = "An Error Occured. Contact Admin";
            return RedirectToAction("Chat", new { id = id });
        }

        // GET: Content/ContactUsChat/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = await db.ContactUs.FindAsync(id);
            if (contactUs == null)
            {
                return NotFound();
            }
            return View(contactUs);
        }

        // POST: Content/ContactUsChat/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("Id,Name,DateCreated,PhoneNumber,Email,Subject,Message,UserId,messageStatus")] ContactUs contactUs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactUs).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(contactUs);
        }

        // GET: Content/ContactUsChat/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = await db.ContactUs.FindAsync(id);
            if (contactUs == null)
            {
                return NotFound();
            }
            return View(contactUs);
        }

        // POST: Content/ContactUsChat/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ContactUs contactUs = await db.ContactUs.FindAsync(id);
            db.ContactUs.Remove(contactUs);
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


