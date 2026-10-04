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
    public class KnowledgeBaseAdminController : Controller
    {
        private readonly MasterDbContext _context;

        public KnowledgeBaseAdminController(MasterDbContext context)
        {
            _context = context;
        }

        // GET: /KnowledgeBaseAdmin
        public async Task<IActionResult> Index(string? role, string? search, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 5) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.KnowledgeBaseItems.AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(k => k.TargetRole == role);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(k =>
                    k.Title.Contains(search) ||
                    k.ControllerName.Contains(search) ||
                    k.ActionName.Contains(search) ||
                    k.RouteUrl.Contains(search) ||
                    k.WhatItDoes.Contains(search) ||
                    (k.BadgeText != null && k.BadgeText.Contains(search)));
            }

            var totalItems = await query.CountAsync();

            var list = await query
                .OrderBy(k => k.TargetRole)
                .ThenBy(k => k.SortOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SelectedRole = role;
            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(list);
        }

        // GET: /KnowledgeBaseAdmin/Create
        public IActionResult Create()
        {
            var model = new MasterKnowledgeBaseItem
            {
                SortOrder = (_context.KnowledgeBaseItems.Any() ? _context.KnowledgeBaseItems.Max(k => k.SortOrder) + 1 : 1),
                IsPublished = true,
                TargetRole = "Admin",
                Area = "Admin",
                ActionName = "Index"
            };
            return View(model);
        }

        // POST: /KnowledgeBaseAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterKnowledgeBaseItem model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.KnowledgeBaseItems.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Knowledge base guide created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /KnowledgeBaseAdmin/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var item = await _context.KnowledgeBaseItems.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /KnowledgeBaseAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, MasterKnowledgeBaseItem model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Knowledge base guide updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.KnowledgeBaseItems.AnyAsync(e => e.Id == model.Id))
                        return NotFound();
                    throw;
                }
            }
            return View(model);
        }

        // POST: /KnowledgeBaseAdmin/TogglePublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(long id)
        {
            var item = await _context.KnowledgeBaseItems.FindAsync(id);
            if (item != null)
            {
                item.IsPublished = !item.IsPublished;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Guide '{item.Title}' status changed to {(item.IsPublished ? "Published" : "Hidden")}.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /KnowledgeBaseAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.KnowledgeBaseItems.FindAsync(id);
            if (item != null)
            {
                _context.KnowledgeBaseItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Knowledge base guide deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
