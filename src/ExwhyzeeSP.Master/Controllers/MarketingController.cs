using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Master.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,ContentManager")]
    public class MarketingController : Controller
    {
        private readonly MasterDbContext _context;
        private readonly IMarketingService _marketingService;
        private readonly IAuditLogService _auditLogService;

        public MarketingController(
            MasterDbContext context,
            IMarketingService marketingService,
            IAuditLogService auditLogService)
        {
            _context = context;
            _marketingService = marketingService;
            _auditLogService = auditLogService;
        }

        // GET: /Marketing
        public async Task<IActionResult> Index()
        {
            var totalContacts = await _context.MarketingContacts.CountAsync();
            var subscribedContacts = await _context.MarketingContacts.CountAsync(c => c.IsSubscribed);
            var totalTemplates = await _context.MarketingTemplates.CountAsync();
            var totalCampaigns = await _context.MarketingCampaigns.CountAsync();
            var totalDelivered = await _context.MarketingCampaigns.SumAsync(c => c.DeliveredCount);

            var recentCampaigns = await _context.MarketingCampaigns
                .Include(c => c.Template)
                .OrderByDescending(c => c.CreatedAtUtc)
                .Take(6)
                .ToListAsync();

            var recentBroadcastEmails = await _context.EmailLogs
                .Where(e => e.EmailCategory == "MarketingBroadcast" || e.EmailCategory == "MarketingTest")
                .OrderByDescending(e => e.CreatedAtUtc)
                .Take(8)
                .ToListAsync();

            var tagStats = await _context.MarketingContacts
                .GroupBy(c => c.Tag)
                .Select(g => new { Tag = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(6)
                .ToListAsync();

            ViewBag.TotalContacts = totalContacts;
            ViewBag.SubscribedContacts = subscribedContacts;
            ViewBag.TotalTemplates = totalTemplates;
            ViewBag.TotalCampaigns = totalCampaigns;
            ViewBag.TotalDelivered = totalDelivered;
            ViewBag.RecentCampaigns = recentCampaigns;
            ViewBag.RecentBroadcastEmails = recentBroadcastEmails;
            ViewBag.TagStats = tagStats;

            return View();
        }

        // GET: /Marketing/Contacts
        public async Task<IActionResult> Contacts(string? tag, string? search, int page = 1)
        {
            const int PageSize = 25;
            IQueryable<MarketingContact> query = _context.MarketingContacts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(tag) && tag != "All")
            {
                query = query.Where(c => c.Tag == tag);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLowerInvariant();
                query = query.Where(c => 
                    c.Email.Contains(s) || 
                    (c.FullName != null && c.FullName.Contains(s)) ||
                    (c.SchoolName != null && c.SchoolName.Contains(s)) ||
                    (c.State != null && c.State.Contains(s)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var contacts = await query
                .OrderByDescending(c => c.CreatedAtUtc)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var distinctTags = await _context.MarketingContacts
                .Select(c => c.Tag)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            ViewBag.Tag = tag ?? "All";
            ViewBag.Search = search ?? string.Empty;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.DistinctTags = distinctTags;

            return View(contacts);
        }

        // POST: /Marketing/AddContact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContact(MarketingContact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Email))
            {
                TempData["ErrorMessage"] = "Email address is required.";
                return RedirectToAction(nameof(Contacts));
            }

            var cleanEmail = contact.Email.Trim().ToLowerInvariant();
            var exists = await _context.MarketingContacts.AnyAsync(c => c.Email.ToLower() == cleanEmail);
            if (exists)
            {
                TempData["ErrorMessage"] = $"Contact with email {cleanEmail} already exists.";
                return RedirectToAction(nameof(Contacts));
            }

            contact.Email = cleanEmail;
            contact.Source = "Manual";
            contact.CreatedAtUtc = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(contact.Tag)) contact.Tag = "General";

            _context.MarketingContacts.Add(contact);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Contact {cleanEmail} added successfully.";
            return RedirectToAction(nameof(Contacts));
        }

        // POST: /Marketing/DeleteContact/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContact(long id)
        {
            var contact = await _context.MarketingContacts.FindAsync(id);
            if (contact != null)
            {
                _context.MarketingContacts.Remove(contact);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact removed from marketing list.";
            }
            return RedirectToAction(nameof(Contacts));
        }

        // POST: /Marketing/ToggleSubscription/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSubscription(long id)
        {
            var contact = await _context.MarketingContacts.FindAsync(id);
            if (contact != null)
            {
                contact.IsSubscribed = !contact.IsSubscribed;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Contact subscription updated to {(contact.IsSubscribed ? "Subscribed" : "Unsubscribed")}.";
            }
            return RedirectToAction(nameof(Contacts));
        }

        // GET: /Marketing/ExportContacts
        public async Task<IActionResult> ExportContacts(string? tag)
        {
            var excelBytes = await _marketingService.ExportContactsExcelAsync(tag);
            var filename = $"Marketing_Audience_{tag ?? "All"}_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        // GET: /Marketing/Import
        public async Task<IActionResult> Import()
        {
            var existingTags = await _context.MarketingContacts
                .Select(c => c.Tag)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            ViewBag.ExistingTags = existingTags;
            return View();
        }

        // POST: /Marketing/ImportExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile? excelFile, string defaultTag = "General", string? newTag = null)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid Excel spreadsheet (.xlsx, .xls, .csv).";
                return RedirectToAction(nameof(Import));
            }

            var targetTag = !string.IsNullOrWhiteSpace(newTag) ? newTag.Trim() : defaultTag;
            using var stream = excelFile.OpenReadStream();
            var result = await _marketingService.ImportContactsFromExcelAsync(stream, targetTag);

            if (result.ErrorMessages.Any())
            {
                TempData["ErrorMessage"] = string.Join("<br>", result.ErrorMessages);
            }
            else
            {
                TempData["SuccessMessage"] = $"Excel Import Completed! Imported: {result.ImportedCount} new, Updated: {result.UpdatedCount} existing, Invalid/Skipped: {result.InvalidCount}.";
                await _auditLogService.LogAsync(
                    action: "MarketingExcelImport",
                    details: $"Imported {result.ImportedCount} new contacts from file '{excelFile.FileName}' into tag '{targetTag}'.",
                    userName: User.Identity?.Name ?? "Admin",
                    area: "Marketing",
                    httpContext: HttpContext);
            }

            return RedirectToAction(nameof(Contacts), new { tag = targetTag });
        }

        // POST: /Marketing/ImportText
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportText(string rawText, string defaultTag = "General", string? newTag = null, string? defaultRole = null, string? defaultSchool = null)
        {
            if (string.IsNullOrWhiteSpace(rawText))
            {
                TempData["ErrorMessage"] = "Please paste email addresses or contact text from your Word / Text document.";
                return RedirectToAction(nameof(Import));
            }

            var targetTag = !string.IsNullOrWhiteSpace(newTag) ? newTag.Trim() : defaultTag;
            var result = await _marketingService.ImportContactsFromRawTextAsync(rawText, targetTag, defaultRole, defaultSchool);

            if (result.ErrorMessages.Any())
            {
                TempData["ErrorMessage"] = string.Join("<br>", result.ErrorMessages);
            }
            else
            {
                TempData["SuccessMessage"] = $"Text Import Completed! Imported: {result.ImportedCount} new, Updated: {result.UpdatedCount} existing, Skipped: {result.InvalidCount}.";
                await _auditLogService.LogAsync(
                    action: "MarketingTextImport",
                    details: $"Imported {result.ImportedCount} contacts via raw text dump into tag '{targetTag}'.",
                    userName: User.Identity?.Name ?? "Admin",
                    area: "Marketing",
                    httpContext: HttpContext);
            }

            return RedirectToAction(nameof(Contacts), new { tag = targetTag });
        }

        // GET: /Marketing/Templates
        public async Task<IActionResult> Templates()
        {
            var templates = await _context.MarketingTemplates
                .OrderByDescending(t => t.IsDefault)
                .ThenBy(t => t.TemplateName)
                .ToListAsync();

            return View(templates);
        }

        // GET: /Marketing/CreateTemplate
        public IActionResult CreateTemplate()
        {
            return View(new MarketingTemplate());
        }

        // POST: /Marketing/CreateTemplate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTemplate(MarketingTemplate model)
        {
            if (string.IsNullOrWhiteSpace(model.TemplateName) || string.IsNullOrWhiteSpace(model.Subject) || string.IsNullOrWhiteSpace(model.BodyHtml))
            {
                TempData["ErrorMessage"] = "Template Name, Subject, and HTML Content are required.";
                return View(model);
            }

            model.CreatedAtUtc = DateTime.UtcNow;
            _context.MarketingTemplates.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Template '{model.TemplateName}' created successfully.";
            return RedirectToAction(nameof(Templates));
        }

        // GET: /Marketing/EditTemplate/{id}
        public async Task<IActionResult> EditTemplate(int id)
        {
            var template = await _context.MarketingTemplates.FindAsync(id);
            if (template == null) return NotFound();
            return View(template);
        }

        // POST: /Marketing/EditTemplate/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTemplate(int id, MarketingTemplate model)
        {
            var template = await _context.MarketingTemplates.FindAsync(id);
            if (template == null) return NotFound();

            if (string.IsNullOrWhiteSpace(model.TemplateName) || string.IsNullOrWhiteSpace(model.Subject) || string.IsNullOrWhiteSpace(model.BodyHtml))
            {
                TempData["ErrorMessage"] = "Template Name, Subject, and HTML Content are required.";
                return View(model);
            }

            template.TemplateName = model.TemplateName;
            template.Subject = model.Subject;
            template.PreheaderText = model.PreheaderText;
            template.BodyHtml = model.BodyHtml;
            template.Category = model.Category;
            template.Description = model.Description;
            template.IsDefault = model.IsDefault;
            template.LastModifiedUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Template '{template.TemplateName}' updated successfully.";
            return RedirectToAction(nameof(Templates));
        }

        // POST: /Marketing/DeleteTemplate/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var template = await _context.MarketingTemplates.FindAsync(id);
            if (template != null)
            {
                _context.MarketingTemplates.Remove(template);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Template deleted.";
            }
            return RedirectToAction(nameof(Templates));
        }

        // GET: /Marketing/GetTemplateJson/{id}
        [HttpGet]
        public async Task<IActionResult> GetTemplateJson(int id)
        {
            var template = await _context.MarketingTemplates.FindAsync(id);
            if (template == null) return NotFound();
            return Json(new
            {
                template.Id,
                template.TemplateName,
                template.Subject,
                template.PreheaderText,
                template.BodyHtml,
                template.Category
            });
        }

        // GET: /Marketing/CreateCampaign
        public async Task<IActionResult> CreateCampaign(int? templateId = null)
        {
            var templates = await _context.MarketingTemplates.OrderBy(t => t.TemplateName).ToListAsync();
            var distinctTags = await _context.MarketingContacts
                .Select(c => c.Tag)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            var totalContacts = await _context.MarketingContacts.CountAsync(c => c.IsSubscribed);

            ViewBag.Templates = templates;
            ViewBag.DistinctTags = distinctTags;
            ViewBag.TotalContacts = totalContacts;

            var campaign = new MarketingCampaign
            {
                Title = $"Marketing Broadcast - {DateTime.UtcNow:MMM d, yyyy}",
                SenderName = "Exwhyzee School Portal",
                SenderEmail = "marketing@exwhyzee.ng",
                TargetTag = "All"
            };

            if (templateId.HasValue)
            {
                var selectedTemplate = templates.FirstOrDefault(t => t.Id == templateId.Value);
                if (selectedTemplate != null)
                {
                    campaign.MarketingTemplateId = selectedTemplate.Id;
                    campaign.Subject = selectedTemplate.Subject;
                    campaign.BodyHtml = selectedTemplate.BodyHtml;
                }
            }
            else if (templates.Any())
            {
                var defaultTemplate = templates.FirstOrDefault(t => t.IsDefault) ?? templates.First();
                campaign.MarketingTemplateId = defaultTemplate.Id;
                campaign.Subject = defaultTemplate.Subject;
                campaign.BodyHtml = defaultTemplate.BodyHtml;
            }

            return View(campaign);
        }

        // POST: /Marketing/CreateCampaign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCampaign(MarketingCampaign model, string submitAction, string? testEmail)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Subject) || string.IsNullOrWhiteSpace(model.BodyHtml))
            {
                TempData["ErrorMessage"] = "Campaign Title, Subject, and Email HTML Content are required.";
                return await CreateCampaign(model.MarketingTemplateId);
            }

            model.CreatedBy = User.Identity?.Name ?? "Admin";
            model.CreatedAtUtc = DateTime.UtcNow;

            if (submitAction == "send_test")
            {
                if (string.IsNullOrWhiteSpace(testEmail))
                {
                    TempData["ErrorMessage"] = "Please specify a test email address.";
                    return await CreateCampaign(model.MarketingTemplateId);
                }

                _context.MarketingCampaigns.Add(model);
                await _context.SaveChangesAsync();

                var testResult = await _marketingService.ExecuteCampaignAsync(model.Id, singleTestEmail: testEmail, operatorUser: User.Identity?.Name);
                TempData["SuccessMessage"] = testResult.Message;
                return RedirectToAction(nameof(CampaignDetails), new { id = model.Id });
            }

            if (submitAction == "launch_now")
            {
                _context.MarketingCampaigns.Add(model);
                await _context.SaveChangesAsync();

                var dispatchResult = await _marketingService.ExecuteCampaignAsync(model.Id, operatorUser: User.Identity?.Name);
                if (dispatchResult.Success)
                {
                    TempData["SuccessMessage"] = dispatchResult.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = dispatchResult.Message;
                }

                return RedirectToAction(nameof(CampaignDetails), new { id = model.Id });
            }

            // Save as draft
            model.Status = "Draft";
            _context.MarketingCampaigns.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Campaign saved as draft.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Marketing/CampaignDetails/{id}
        public async Task<IActionResult> CampaignDetails(long id)
        {
            var campaign = await _context.MarketingCampaigns
                .Include(c => c.Template)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (campaign == null) return NotFound();

            var emailLogs = await _context.EmailLogs
                .Where(e => (e.EmailCategory == "MarketingBroadcast" || e.EmailCategory == "MarketingTest") && e.Subject.Contains(campaign.Subject))
                .OrderByDescending(e => e.CreatedAtUtc)
                .Take(50)
                .ToListAsync();

            ViewBag.EmailLogs = emailLogs;
            return View(campaign);
        }
    }
}
