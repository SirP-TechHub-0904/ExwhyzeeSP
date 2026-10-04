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
    public class RoomsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IAccomodationService _accomodationService = new AccomodationService();
        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public RoomsController()
        {

        }

        public RoomsController(AccomodationService accomodationService)
        {
            _accomodationService = accomodationService;
        }

        // GET: Accomodation/Rooms
        public async Task<ActionResult> Index()
        {
            var item = await _accomodationService.HostelRoomList();
            return View(item);
        }

        // GET: Accomodation/Rooms/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelRoom = await _accomodationService.GetHostelRoom(id);
            if (hostelRoom == null)
            {
                return NotFound();
            }
            return View(hostelRoom);
        }

        // GET: Accomodation/Rooms/Create
        public ActionResult Create()
        {
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name");
            return View();
        }

        // POST: Accomodation/Rooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HostelRoom hostelRoom)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.AddRoom(hostelRoom);
                TempData["success"] = "Hostel Room Added Successfully";
                return RedirectToAction("Index");
            }

            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", hostelRoom.HostelId);
            TempData["error"] = "Unable to add Hostel Room";
            return View(hostelRoom);
        }

        // GET: Accomodation/Rooms/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelRoom = await _accomodationService.GetHostelRoom(id);
            if (hostelRoom == null)
            {
                return NotFound();
            }
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", hostelRoom.HostelId);
            return View(hostelRoom);
        }

        // POST: Accomodation/Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HostelRoom hostelRoom)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.EditHostelRoom(hostelRoom);
                TempData["success"] = "Hostel Room Edited Successfully";
                return RedirectToAction("Index");
            }
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", hostelRoom.HostelId);
            TempData["error"] = "Unable to Edit Room!";
            return View(hostelRoom);
        }

        // GET: Accomodation/Rooms/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelRoom = await _accomodationService.GetHostelRoom(id);
            if (hostelRoom == null)
            {
                return NotFound();
            }
            return View(hostelRoom);
        }

        // POST: Accomodation/Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _accomodationService.DeleteHostelRoom(id);
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


