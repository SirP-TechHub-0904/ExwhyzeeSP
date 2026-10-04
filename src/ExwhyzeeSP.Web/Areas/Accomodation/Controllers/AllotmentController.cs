using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Web.Areas.Data.Services;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExwhyzeeSP.Web.Areas.Accomodation.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Student")]
    [Area("Accomodation")]
    public class AllotmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IAccomodationService _accomodationService = new AccomodationService();

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public AllotmentController()
        {
        }

        public AllotmentController(AccomodationService accomodationService)
        {
            _accomodationService = accomodationService;
        }
        // GET: Accomodation/Allotment
        public async Task<ActionResult> Index()
        {
            var item = await _accomodationService.HostelAllotmentList();
            return View(item);
        }


        // GET: Accomodation/MyAccomodation
        public async Task<ActionResult> MyAccomodation()
        {
            var item = await _accomodationService.StudentHostelAllotmentList();
            return View(item);
        }

        // GET: Accomodation/Allotment/Allocate
        public ActionResult Allocate(int? fid)
        {
            ViewBag.id = fid;
            return View();
        }

        // POST: Accomodation/Allotment/Allocate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Allocate(HostelAllotment allotment, int? fid)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.AddHostelAllotment(allotment,fid);
                TempData["success"] = "Student Allocated Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Allocate to a hostel, it might be that there is no more available space. Please contact the Admin for more information";
            return RedirectToAction("Index");
        }

        // GET: Accomodation/Allotment/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelBed = await _accomodationService.GetHostelAllotment(id);
            if (hostelBed == null)
            {
                return NotFound();
            }
            return View(hostelBed);
        }

        // GET: Accomodation/Allotment/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var allotment = await _accomodationService.GetHostelAllotment(id);
            if (allotment == null)
            {
                return NotFound();
            }
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", allotment.HostelId);
            return View(allotment);
        }

        // POST: Accomodation/Allotment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HostelAllotment allotment)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.EditHostelAllotment(allotment);
                TempData["success"] = "Allotment Edited Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Edit Allotment";
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", allotment.HostelId);
            return View(allotment);
        }

        // GET: Accomodation/Allotment/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelBed = await _accomodationService.GetHostelAllotment(id);
            if (hostelBed == null)
            {
                return NotFound();
            }
            return View(hostelBed);
        }

        // POST: Accomodation/Allotment/Delete/5
        [HttpPost, ActionName("Delete")]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _accomodationService.DeleteHostelAllotment(id);
            TempData["success"] = "Allotment Deleted Successfully";
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

        public JsonResult BedList(int Id)
        {
            if (Id <= 0)
            {
                return Json(new SelectList(new List<HostelBed>(), "Id", "BedNo"));
            }
            var hr = db.HostelRooms.FirstOrDefault(x => x.Id == Id);
            if (hr == null)
            {
                return Json(new SelectList(new List<HostelBed>(), "Id", "BedNo"));
            }
            var roomId = hr.Id;
            var room = from s in db.HostelBeds
                       where s.HostelRoomId == roomId
                       select s;

            return Json(new SelectList(room.ToArray(), "Id", "BedNo"));
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

