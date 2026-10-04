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
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin},{MasterRoles.ContentManager}")]
    public class TestimoniesController : Controller
    {
        private readonly MasterDbContext _context;

        public TestimoniesController(MasterDbContext context)
        {
            _context = context;
        }

        // GET: /Testimonies
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 5) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Testimonies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(t =>
                    t.Title.Contains(search) ||
                    (t.SchoolName != null && t.SchoolName.Contains(search)) ||
                    (t.Position != null && t.Position.Contains(search)) ||
                    t.Description.Contains(search));
            }

            var totalItems = await query.CountAsync();

            var list = await query
                .OrderBy(t => t.SortOrder)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(list);
        }

        // GET: /Testimonies/Create
        public IActionResult Create()
        {
            return View(new Testimony { Show = true, Rating = 5, SortOrder = "1" });
        }

        // POST: /Testimonies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Testimony model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Testimonies.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Testimonial created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Testimonies/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var item = await _context.Testimonies.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Testimonies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Testimony model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Testimonial updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Testimonies.AnyAsync(e => e.Id == model.Id))
                        return NotFound();
                    throw;
                }
            }
            return View(model);
        }

        // POST: /Testimonies/ToggleShow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleShow(long id)
        {
            var item = await _context.Testimonies.FindAsync(id);
            if (item != null)
            {
                item.Show = !item.Show;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Testimonial visibility changed to {(item.Show ? "Published" : "Hidden")}.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Testimonies/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.Testimonies.FindAsync(id);
            if (item != null)
            {
                _context.Testimonies.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Testimonial deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
