using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Master.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin},{MasterRoles.Accountant}")]
    public class PricingPlansController : Controller
    {
        private readonly MasterDbContext _context;

        public PricingPlansController(MasterDbContext context)
        {
            _context = context;
        }

        // GET: /PricingPlans
        public async Task<IActionResult> Index()
        {
            var plans = await _context.PricingPlans
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Id)
                .ToListAsync();
            return View(plans);
        }

        // GET: /PricingPlans/Create
        public IActionResult Create()
        {
            var model = new MasterPricingPlan
            {
                SortOrder = (_context.PricingPlans.Any() ? _context.PricingPlans.Max(p => p.SortOrder) + 1 : 1),
                IsActive = true,
                ButtonText = "Get Started",
                ButtonUrl = "/Demo"
            };
            return View(model);
        }

        // POST: /PricingPlans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterPricingPlan model)
        {
            if (ModelState.IsValid)
            {
                _context.PricingPlans.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Pricing plan created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /PricingPlans/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var plan = await _context.PricingPlans.FindAsync(id);
            if (plan == null) return NotFound();
            return View(plan);
        }

        // POST: /PricingPlans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, MasterPricingPlan model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Pricing plan updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.PricingPlans.AnyAsync(e => e.Id == model.Id))
                        return NotFound();
                    throw;
                }
            }
            return View(model);
        }

        // POST: /PricingPlans/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(long id)
        {
            var plan = await _context.PricingPlans.FindAsync(id);
            if (plan != null)
            {
                plan.IsActive = !plan.IsActive;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Plan '{plan.Name}' status updated to {(plan.IsActive ? "Active" : "Inactive")}.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /PricingPlans/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var plan = await _context.PricingPlans.FindAsync(id);
            if (plan != null)
            {
                _context.PricingPlans.Remove(plan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Pricing plan deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
