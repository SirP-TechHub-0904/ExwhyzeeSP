using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Web.Areas.Data.Services;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;

namespace ExwhyzeeSP.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("Admin")]
    public class NewsLettersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IGeneralRemarkService _generalRemarkService = new GeneralRemarkService();
        private ISessionService _sessionService = new SessionService();


        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public NewsLettersController()
        {

        }
        public NewsLettersController(GeneralRemarkService generalRemarkService, SessionService sessionService)
        {
            _generalRemarkService = generalRemarkService;
            _sessionService = sessionService;

        }
        // GET: Admin/ SchoolAccounts
        public async Task<ActionResult> Index()
        {
            var items = await _generalRemarkService.List();
          
            return View(items);
        }

        // GET: Admin/SchoolAccounts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var fee = await _generalRemarkService.Get(id);
            if (fee == null)
            {
                return NotFound();
            }
            return View(fee);
        }

        // GET: Admin/SchoolAccounts/Create
        public async Task<ActionResult> Create()
        {
            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
            return View();
        }

        // POST: Admin/SchoolAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewsLetter news)
        {
            if (ModelState.IsValid)
            {
                await _generalRemarkService.Create(news);

                return RedirectToAction("Index");
            }


            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession", news.SessionId);
            return View(news);
        }

        // GET: Admin/SchoolAccounts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var news = await _generalRemarkService.Get(id);
            if (news == null)
            {
                return NotFound();
            }
            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession", news.SessionId);
            return View(news);
        }

        // POST: Admin/SchoolAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(NewsLetter news)
        {
            if (ModelState.IsValid)
            {
                await _generalRemarkService.Edit(news);
                return RedirectToAction("Index");
            }
            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession", news.SessionId);
            return View(news);
        }

        // GET: Admin/SchoolAccounts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var news = await _generalRemarkService.Get(id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: Admin/SchoolAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _generalRemarkService.Delete(id);

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


