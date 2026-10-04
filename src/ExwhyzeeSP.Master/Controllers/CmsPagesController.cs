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
    public class CmsPagesController : Controller
    {
        private readonly MasterDbContext _context;

        public CmsPagesController(MasterDbContext context)
        {
            _context = context;
        }

        // GET: /CmsPages
        public async Task<IActionResult> Index()
        {
            var pages = await _context.MasterCmsPages
                .OrderBy(p => p.Title)
                .ToListAsync();
            return View(pages);
        }

        // GET: /CmsPages/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var page = await _context.MasterCmsPages.FindAsync(id);
            if (page == null) return NotFound();
            return View(page);
        }

        // POST: /CmsPages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, MasterCmsPage model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    model.LastUpdated = DateTime.UtcNow;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"'{model.Title}' updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.MasterCmsPages.AnyAsync(e => e.Id == model.Id))
                        return NotFound();
                    throw;
                }
            }
            return View(model);
        }
    }
}
