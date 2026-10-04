using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ExwhyzeeSP.Web.Areas.Data.Services;
using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExwhyzeeSP.Web.Areas.Financial.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finance")]
    [Area("Financial")]
    public class ExpenditureSourceController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IExpenditureService _expService = new ExpenditureService();

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public ExpenditureSourceController()
        {

        }

        public ExpenditureSourceController(
            ExpenditureService expService
            )
        {
            _expService = expService;
        }
        // GET: Financial/Expenditure
        public async Task<ActionResult> Index()
        {
            var model = await _expService.ExpenditureList();
            return View(model);
        }

        // GET: Financial/Expenditure/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _expService.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // GET: Financial/Expenditure/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Financial/Expenditure/Create
        [HttpPost]
        public async Task<ActionResult> Create(Expenditure item)
        {

            if (ModelState.IsValid)
            {
                //string uId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //item.Date = DateTime.UtcNow.AddHours(1);
                //item.UserId = uId;
                //db.Expenditures.Add(item);
                //await db.SaveChangesAsync();
                await _expService.Create(item);
                TempData["success"] = "Expenditure Added Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Add Expenditure";
            return View();

        }

        // GET: Financial/Expenditure/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _expService.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Financial/Expenditure/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Expenditure item)
        {
            if (ModelState.IsValid)
            {
                await _expService.Edit(item);
                TempData["success"] = "Expenditure Edited Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Edit Expenditure";
            return View(item);
        }

        // GET: Financial/Expenditure/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _expService.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Financial/Expenditure/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _expService.Delete(id);
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


