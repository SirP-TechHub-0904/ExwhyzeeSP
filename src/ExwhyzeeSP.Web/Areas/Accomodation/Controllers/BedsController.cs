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
using ExwhyzeeSP.Web.Areas.Data.Services;
using ExwhyzeeSP.Web.Areas.Data.IServices;

namespace ExwhyzeeSP.Web.Areas.Accomodation.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("Accomodation")]
    public class BedsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IAccomodationService _accomodationService = new AccomodationService();

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public BedsController()
        {

        }

        public BedsController(AccomodationService accomodationService)
        {
            _accomodationService = accomodationService;
        }
        // GET: Accomodation/Beds
        public async Task<ActionResult> Index()
        {
            var hostelBeds = await _accomodationService.HostelBedList();
            return View(hostelBeds);
        }

        // GET: Accomodation/Beds/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelBed = await _accomodationService.GetHostelBed(id);
            if (hostelBed == null)
            {
                return NotFound();
            }
            return View(hostelBed);
        }

        // GET: Accomodation/Beds/Create
        public ActionResult Create()
        {
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name");
            return View();
        }

        // POST: Accomodation/Beds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HostelBed hostelBed)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.AddHostelBed(hostelBed);
                TempData["success"] = "Bed Added Successfully";
                return RedirectToAction("Index");
            }

            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", hostelBed.HostelId);
            TempData["error"] = "Unable to Add Bed";
            return View(hostelBed);
        }

        // GET: Accomodation/Beds/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelBed = await _accomodationService.GetHostelBed(id);
            if (hostelBed == null)
            {
                return NotFound();
            }
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", hostelBed.HostelId);
            return View(hostelBed);
        }

        // POST: Accomodation/Beds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HostelBed hostelBed)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.EditHostelBed(hostelBed);
                TempData["success"] = "Bed Edited Successfully";
                return RedirectToAction("Index");
            }
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", hostelBed.HostelId);
            TempData["error"] = "Unable to Edit Room";
            return View(hostelBed);
        }

        // GET: Accomodation/Beds/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelBed = await _accomodationService.GetHostelBed(id);
            if (hostelBed == null)
            {
                return NotFound();
            }
            return View(hostelBed);
        }

        // POST: Accomodation/Beds/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _accomodationService.DeleteHostelBed(id);
            TempData["success"] = "Bed Deleted Successfully";
            return RedirectToAction("Index");
        }


        public JsonResult RoomList(int Id)
        {
            if (Id <= 0)
            {
                return Json(new SelectList(new List<HostelRoom>(), "Id", "Name"));
            }
            var hostel = db.Hostels.FirstOrDefault(x => x.Id == Id);
            if (hostel == null)
            {
                return Json(new SelectList(new List<HostelRoom>(), "Id", "Name"));
            }
            var hostelId = hostel.Id;
            var room = from s in db.HostelRooms
                        where s.HostelId == hostelId
                        select s;

            return Json(new SelectList(room.ToArray(), "Id", "Name"));
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


