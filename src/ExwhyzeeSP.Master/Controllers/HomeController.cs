using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Master.Models;
using ExwhyzeeSP.Core.Helpers;

namespace ExwhyzeeSP.Master.Controllers
{
    /// <summary>
    /// Public marketing website for the ExwhyzeeSP platform (Vistara template).
    /// The Master Admin area lives under /Dashboard and /Tenants and requires login.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly MasterDbContext _context;

        public HomeController(MasterDbContext context)
        {
            _context = context;
        }

        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            var testimonies = await _context.Testimonies
                .Where(t => t.Show)
                .OrderBy(t => t.SortOrder)
                .Take(6)
                .ToListAsync();
            return View(testimonies);
        }

        [HttpGet("/About")]
        public IActionResult About() => View();

        [HttpGet("/Services")]
        public IActionResult Services() => View();

        [HttpGet("/Pricing")]
        public async Task<IActionResult> Pricing()
        {
            var plans = await _context.PricingPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();
            return View(plans);
        }

        [HttpGet("/Demo")]
        public IActionResult Demo() => View();

        [HttpGet("/Team")]
        public IActionResult Team() => View();

        [HttpGet("/Testimonials")]
        public async Task<IActionResult> Testimonials()
        {
            var testimonies = await _context.Testimonies
                .Where(t => t.Show)
                .OrderBy(t => t.SortOrder)
                .ToListAsync();
            return View(testimonies);
        }

        [HttpGet("/Faq")]
        public IActionResult Faq() => View();

        [HttpGet("/KnowledgeBase")]
        [HttpGet("/Help")]
        public async Task<IActionResult> KnowledgeBase(string? role, string? search)
        {
            var query = _context.KnowledgeBaseItems.Where(k => k.IsPublished).AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(k => k.TargetRole == role);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(k =>
                    k.Title.Contains(search) ||
                    k.ControllerName.Contains(search) ||
                    k.ActionName.Contains(search) ||
                    k.RouteUrl.Contains(search) ||
                    k.WhatItDoes.Contains(search));
            }

            var items = await query
                .OrderBy(k => k.TargetRole)
                .ThenBy(k => k.SortOrder)
                .ToListAsync();

            ViewBag.SelectedRole = role;
            ViewBag.Search = search;
            return View(items);
        }

        [HttpGet("/Contact")]
        public IActionResult Contact() => View();

        [HttpGet("/Privacy")]
        [HttpGet("/PrivacyPolicy")]
        public async Task<IActionResult> Privacy()
        {
            var page = await _context.MasterCmsPages.FirstOrDefaultAsync(p => p.Slug == "privacy-policy");
            ViewData["ActiveSlug"] = "privacy-policy";
            return View("LegalPage", page);
        }

        [HttpGet("/Terms")]
        [HttpGet("/TermsAndConditions")]
        public async Task<IActionResult> Terms()
        {
            var page = await _context.MasterCmsPages.FirstOrDefaultAsync(p => p.Slug == "terms-and-conditions");
            ViewData["ActiveSlug"] = "terms-and-conditions";
            return View("LegalPage", page);
        }

        [HttpGet("/Cookies")]
        [HttpGet("/CookiePolicy")]
        public async Task<IActionResult> Cookies()
        {
            var page = await _context.MasterCmsPages.FirstOrDefaultAsync(p => p.Slug == "cookie-policy");
            ViewData["ActiveSlug"] = "cookie-policy";
            return View("LegalPage", page);
        }

        [HttpGet("/Blog")]
        [HttpGet("/News")]
        [HttpGet("/Blog/Category/{categoryId:long}/{slug?}")]
        [HttpGet("/Blog/Category/{categoryId:long}")]
        public async Task<IActionResult> Blog(long? categoryId, string? search, string? slug = null)
        {
            var query = _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(b => b.Publish)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(b => b.BlogCategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(s) || b.Description.ToLower().Contains(s));
            }

            var blogs = await query.OrderByDescending(b => b.Date).ToListAsync();
            ViewBag.Categories = await _context.BlogCategories.Where(c => c.Publish).ToListAsync();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Search = search;

            return View(blogs);
        }

        [HttpGet("/Blog/{id:long}/{slug?}")]
        [HttpGet("/Blog/{id:long}")]
        [HttpGet("/Blog/Article/{id:long}/{slug?}")]
        [HttpGet("/Blog/Article/{id:long}")]
        public async Task<IActionResult> BlogDetail(long id, string? slug = null)
        {
            var blog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .FirstOrDefaultAsync(b => b.Id == id && b.Publish);

            if (blog == null)
            {
                return RedirectToAction(nameof(Blog));
            }

            var related = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Where(b => b.Publish && b.Id != id)
                .OrderByDescending(b => b.Date)
                .Take(3)
                .ToListAsync();

            ViewBag.RelatedBlogs = related;
            ViewBag.Categories = await _context.BlogCategories.Where(c => c.Publish).ToListAsync();

            return View(blog);
        }

        [HttpGet("/robots.txt")]
        public IActionResult Robots()
        {
            var host = Request.Scheme + "://" + Request.Host;
            var robots = $@"User-agent: *
Allow: /
Disallow: /Dashboard
Disallow: /Tenants
Disallow: /Testimonies
Disallow: /CmsPages
Disallow: /BlogPosts
Disallow: /Account/Login
Disallow: /Admin/

Sitemap: {host}/sitemap.xml
";
            return Content(robots, "text/plain", System.Text.Encoding.UTF8);
        }

        [HttpGet("/sitemap.xml")]
        public async Task<IActionResult> SitemapXml()
        {
            var host = Request.Scheme + "://" + Request.Host;
            var blogs = await _context.Blogs.Where(b => b.Publish).ToListAsync();
            var pages = await _context.MasterCmsPages.Where(p => p.IsPublished).ToListAsync();

            var staticUrls = new[]
            {
                new { Loc = $"{host}/", Priority = "1.0", ChangeFreq = "weekly" },
                new { Loc = $"{host}/About", Priority = "0.9", ChangeFreq = "monthly" },
                new { Loc = $"{host}/Services", Priority = "0.9", ChangeFreq = "monthly" },
                new { Loc = $"{host}/Pricing", Priority = "0.9", ChangeFreq = "weekly" },
                new { Loc = $"{host}/Blog", Priority = "0.8", ChangeFreq = "daily" },
                new { Loc = $"{host}/Testimonials", Priority = "0.8", ChangeFreq = "monthly" },
                new { Loc = $"{host}/KnowledgeBase", Priority = "0.8", ChangeFreq = "monthly" },
                new { Loc = $"{host}/Contact", Priority = "0.8", ChangeFreq = "monthly" },
                new { Loc = $"{host}/Privacy", Priority = "0.5", ChangeFreq = "yearly" },
                new { Loc = $"{host}/Terms", Priority = "0.5", ChangeFreq = "yearly" },
                new { Loc = $"{host}/Cookies", Priority = "0.5", ChangeFreq = "yearly" },
                new { Loc = $"{host}/Sitemap", Priority = "0.6", ChangeFreq = "weekly" }
            };

            var xml = new System.Text.StringBuilder();
            xml.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
            xml.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

            foreach (var item in staticUrls)
            {
                xml.AppendLine($@"  <url>
    <loc>{item.Loc}</loc>
    <changefreq>{item.ChangeFreq}</changefreq>
    <priority>{item.Priority}</priority>
  </url>");
            }

            foreach (var b in blogs)
            {
                var slug = UrlSlugHelper.ToSlug(b.Title);
                xml.AppendLine($@"  <url>
    <loc>{host}/Blog/{b.Id}/{slug}</loc>
    <lastmod>{b.Date:yyyy-MM-dd}</lastmod>
    <changefreq>weekly</changefreq>
    <priority>0.7</priority>
  </url>");
            }

            xml.AppendLine(@"</urlset>");

            return Content(xml.ToString(), "application/xml", System.Text.Encoding.UTF8);
        }

        [HttpGet("/Sitemap")]
        public async Task<IActionResult> Sitemap()
        {
            var blogs = await _context.Blogs.Where(b => b.Publish).OrderByDescending(b => b.Date).Take(10).ToListAsync();
            var categories = await _context.BlogCategories.Where(c => c.Publish).ToListAsync();
            ViewBag.RecentBlogs = blogs;
            ViewBag.Categories = categories;
            return View();
        }

        [HttpGet("/Home/NotFound")]
        public IActionResult NotFound404()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
