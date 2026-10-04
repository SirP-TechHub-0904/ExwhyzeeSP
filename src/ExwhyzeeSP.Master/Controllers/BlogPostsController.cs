using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Master.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin},{MasterRoles.ContentManager}")]
    public class BlogPostsController : Controller
    {
        private readonly MasterDbContext _context;

        public BlogPostsController(MasterDbContext context)
        {
            _context = context;
        }

        // GET: /BlogPosts
        public async Task<IActionResult> Index(string? search, long? categoryId, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 5) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Blogs
                .Include(b => b.BlogCategory)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(b =>
                    b.Title.Contains(search) ||
                    b.Description.Contains(search) ||
                    (b.RibonCustomDisplayTitle != null && b.RibonCustomDisplayTitle.Contains(search)) ||
                    (b.BlogCategory != null && b.BlogCategory.Title.Contains(search)));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(b => b.BlogCategoryId == categoryId.Value);
            }

            var totalItems = await query.CountAsync();

            var blogs = await query
                .OrderByDescending(b => b.Date)
                .ThenBy(b => b.SortOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = await _context.BlogCategories.Where(c => c.Publish).OrderBy(c => c.Title).ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(blogs);
        }

        // GET: /BlogPosts/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.BlogCategories.Where(c => c.Publish).ToListAsync(), "Id", "Title");
            return View(new Blog { Date = DateTime.UtcNow, Publish = true, SortOrder = 1 });
        }

        // POST: /BlogPosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog model)
        {
            if (ModelState.IsValid)
            {
                if (model.Date == default) model.Date = DateTime.UtcNow;
                _context.Blogs.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Blog article published successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _context.BlogCategories.Where(c => c.Publish).ToListAsync(), "Id", "Title", model.BlogCategoryId);
            return View(model);
        }

        // GET: /BlogPosts/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var item = await _context.Blogs.FindAsync(id);
            if (item == null) return NotFound();
            ViewBag.Categories = new SelectList(await _context.BlogCategories.Where(c => c.Publish).ToListAsync(), "Id", "Title", item.BlogCategoryId);
            return View(item);
        }

        // POST: /BlogPosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Blog model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Blog article updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Blogs.AnyAsync(e => e.Id == model.Id))
                        return NotFound();
                    throw;
                }
            }
            ViewBag.Categories = new SelectList(await _context.BlogCategories.Where(c => c.Publish).ToListAsync(), "Id", "Title", model.BlogCategoryId);
            return View(model);
        }

        // POST: /BlogPosts/TogglePublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(long id)
        {
            var item = await _context.Blogs.FindAsync(id);
            if (item != null)
            {
                item.Publish = !item.Publish;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Article publish status changed to {(item.Publish ? "Published" : "Draft")}.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /BlogPosts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.Blogs.FindAsync(id);
            if (item != null)
            {
                _context.Blogs.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Blog article deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
