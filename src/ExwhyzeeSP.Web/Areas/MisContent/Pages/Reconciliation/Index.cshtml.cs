using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.Reconciliation
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;

        public IndexModel(
            ApplicationDbContext context,
            IStorageService storageService,
            IConfiguration config)
        {
            _context = context;
            _storageService = storageService;
            _config = config;
        }

        public bool HasAwsCredentials { get; set; }
        public string BucketName { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public List<ReconciliationItem> Items { get; set; } = new();

        public class ReconciliationItem
        {
            public string Table { get; set; } = string.Empty;
            public long RecordId { get; set; }
            public string ColumnPair { get; set; } = string.Empty;
            public string Key { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? msg = null, string? err = null)
        {
            if (!string.IsNullOrWhiteSpace(msg)) StatusMessage = msg;
            if (!string.IsNullOrWhiteSpace(err)) ErrorMessage = err;

            await LoadAwsStateAsync();
            await LoadItemsAsync();
        }

        public async Task<IActionResult> OnPostReassignOneAsync(string table, long recordId, string columnPair, string key)
        {
            await LoadAwsStateAsync();

            if (!HasAwsCredentials)
            {
                return RedirectToPage(new { err = "AWS credentials (AwsConfiguration__AWSAccessKey / AWSSecretKey) are not configured in .env." });
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return RedirectToPage(new { err = $"No Key found for {table} #{recordId} ({columnPair})." });
            }

            var creds = GetCredentials();
            var res = await _storageService.GetUrlAsync(creds, key.Trim());
            if (string.IsNullOrWhiteSpace(res.Url))
            {
                return RedirectToPage(new { err = $"Failed to generate URL for {table} #{recordId}: {res.Message}" });
            }

            var updated = await UpdateSingleRecordAsync(table, recordId, columnPair, key.Trim(), res.Url);
            if (!updated)
            {
                return RedirectToPage(new { err = $"Record {table} #{recordId} ({columnPair}) was not found." });
            }

            return RedirectToPage(new { msg = $"Reassigned {table} #{recordId} ({columnPair}) using key '{key}'." });
        }

        private AwsCredentials GetCredentials() => new AwsCredentials
        {
            AccessKey = _config["AwsConfiguration:AWSAccessKey"] ?? Environment.GetEnvironmentVariable("AwsConfiguration__AWSAccessKey"),
            SecretKey = _config["AwsConfiguration:AWSSecretKey"] ?? Environment.GetEnvironmentVariable("AwsConfiguration__AWSSecretKey")
        };

        private async Task LoadAwsStateAsync()
        {
            var creds = GetCredentials();
            HasAwsCredentials = !string.IsNullOrWhiteSpace(creds.AccessKey) && !string.IsNullOrWhiteSpace(creds.SecretKey);
            BucketName = await _storageService.BucketName();
        }

        private async Task LoadItemsAsync()
        {
            Items.Clear();

            foreach (var s in await _context.Sliders.AsNoTracking().ToListAsync())
            {
                AddIfHasKey("Sliders", s.Id, "Key/Url", s.Key, s.Url);
                AddIfHasKey("Sliders", s.Id, "SecondKey/SecondUrl", s.SecondKey, s.SecondUrl);
            }

            foreach (var p in await _context.WebPages.AsNoTracking().ToListAsync())
            {
                AddIfHasKey("WebPages", p.Id, "ImageKey/ImageUrl", p.ImageKey, p.ImageUrl);
            }

            foreach (var sec in await _context.PageSections.AsNoTracking().ToListAsync())
            {
                AddIfHasKey("PageSections", sec.Id, "ImageKey/ImageUrl", sec.ImageKey, sec.ImageUrl);
                AddIfHasKey("PageSections", sec.Id, "SecondImageKey/SecondImageUrl", sec.SecondImageKey, sec.SecondImageUrl);
                AddIfHasKey("PageSections", sec.Id, "VideoKey/VideoUrl", sec.VideoKey, sec.VideoUrl);
            }

            foreach (var sl in await _context.PageSectionLists.AsNoTracking().ToListAsync())
            {
                AddIfHasKey("PageSectionLists", sl.Id, "ImageKey/ImageUrl", sl.ImageKey, sl.ImageUrl);
                AddIfHasKey("PageSectionLists", sl.Id, "VideoKey/VideoUrl", sl.VideoKey, sl.VideoUrl);
            }

            foreach (var b in await _context.Blogs.AsNoTracking().ToListAsync())
            {
                AddIfHasKey("Blogs", b.Id, "ImageKey/ImageUrl", b.ImageKey, b.ImageUrl);
                AddIfHasKey("Blogs", b.Id, "VideoKey/VideoUrl", b.VideoKey, b.VideoUrl);
            }

            foreach (var t in await _context.Testimonies.AsNoTracking().ToListAsync())
            {
                AddIfHasKey("Testimonies", t.Id, "ImageKey/ImageUrl", t.ImageKey, t.ImageUrl);
            }

            foreach (var m in await _context.PostModal.AsNoTracking().ToListAsync())
            {
                AddIfHasKey("PostModal", m.Id, "ImageKey/ImageUrl", m.ImageKey, m.ImageUrl);
            }

            foreach (var ss in await _context.SuperSettings.AsNoTracking().ToListAsync())
            {
                AddIfHasKey("SuperSettings", ss.Id, "CompanyLogoKey/Url", ss.CompanyLogoKey, ss.CompanyLogoUrl);
                AddIfHasKey("SuperSettings", ss.Id, "CompanyWhiteLogoKey/Url", ss.CompanyWhiteLogoKey, ss.CompanyWhiteLogoUrl);
                AddIfHasKey("SuperSettings", ss.Id, "CompanyIconKey/Url", ss.CompanyIconKey, ss.CompanyIconUrl);
                AddIfHasKey("SuperSettings", ss.Id, "LoginBackgroundImageKey/Url", ss.LoginBackgroundImageKey, ss.LoginBackgroundImageUrl);
                AddIfHasKey("SuperSettings", ss.Id, "ReloaderIconKey/Url", ss.ReloaderIconKey, ss.ReloaderIconUrl);
                AddIfHasKey("SuperSettings", ss.Id, "PlayStoreImageKey/Url", ss.PlayStoreImageKey, ss.PlayStoreImageUrl);
                AddIfHasKey("SuperSettings", ss.Id, "AppStoreImageKey/Url", ss.AppStoreImageKey, ss.AppStoreImageUrl);
                AddIfHasKey("SuperSettings", ss.Id, "PortfolioBreacrumImageKey/Url", ss.PortfolioBreacrumImageKey, ss.PortfolioBreacrumImageUrl);
                AddIfHasKey("SuperSettings", ss.Id, "PortfolioImageOneKey/Url", ss.PortfolioImageOneKey, ss.PortfolioImageOneUrl);
                AddIfHasKey("SuperSettings", ss.Id, "PortfolioImageTwoKey/Url", ss.PortfolioImageTwoKey, ss.PortfolioImageTwoUrl);
            }

            foreach (var st in await _context.Settings.AsNoTracking().ToListAsync())
            {
                AddIfHasKey("Settings", st.Id, "DefaultTitleBackgroundKey/Url", st.DefaultTitleBackgroundKey, st.DefaultTitleBackgroundUrl);
                AddIfHasKey("Settings", st.Id, "JobCardTitleBackgroundKey/Url", st.JobCardTitleBackgroundKey, st.JobCardTitleBackgroundUrl);
                AddIfHasKey("Settings", st.Id, "SignatureKey/Url", st.SignatureKey, st.SignatureUrl);
            }
        }

        private void AddIfHasKey(string table, long id, string columnPair, string? key, string? url)
        {
            var resolvedKey = key?.Trim();
            if (string.IsNullOrWhiteSpace(resolvedKey) && !string.IsNullOrWhiteSpace(url))
            {
                var clean = url.Split('?')[0].Trim();
                var slash = clean.LastIndexOf('/');
                if (slash >= 0 && slash < clean.Length - 1)
                {
                    resolvedKey = clean.Substring(slash + 1);
                }
            }

            if (string.IsNullOrWhiteSpace(resolvedKey))
                return;

            Items.Add(new ReconciliationItem
            {
                Table = table,
                RecordId = id,
                ColumnPair = columnPair,
                Key = resolvedKey,
                Url = url ?? string.Empty
            });
        }

        private async Task<bool> UpdateSingleRecordAsync(string table, long recordId, string columnPair, string key, string newUrl)
        {
            switch (table)
            {
                case "Sliders":
                    var s = await _context.Sliders.FindAsync(recordId);
                    if (s == null) return false;
                    if (columnPair == "Key/Url") s.Url = newUrl;
                    else if (columnPair == "SecondKey/SecondUrl") s.SecondUrl = newUrl;
                    else return false;
                    break;

                case "WebPages":
                    var p = await _context.WebPages.FindAsync(recordId);
                    if (p == null) return false;
                    p.ImageUrl = newUrl;
                    break;

                case "PageSections":
                    var sec = await _context.PageSections.FindAsync(recordId);
                    if (sec == null) return false;
                    if (columnPair == "ImageKey/ImageUrl") sec.ImageUrl = newUrl;
                    else if (columnPair == "SecondImageKey/SecondImageUrl") sec.SecondImageUrl = newUrl;
                    else if (columnPair == "VideoKey/VideoUrl") sec.VideoUrl = newUrl;
                    else return false;
                    break;

                case "PageSectionLists":
                    var sl = await _context.PageSectionLists.FindAsync(recordId);
                    if (sl == null) return false;
                    if (columnPair == "ImageKey/ImageUrl") sl.ImageUrl = newUrl;
                    else if (columnPair == "VideoKey/VideoUrl") sl.VideoUrl = newUrl;
                    else return false;
                    break;

                case "Blogs":
                    var b = await _context.Blogs.FindAsync(recordId);
                    if (b == null) return false;
                    if (columnPair == "ImageKey/ImageUrl") b.ImageUrl = newUrl;
                    else if (columnPair == "VideoKey/VideoUrl") b.VideoUrl = newUrl;
                    else return false;
                    break;

                case "Testimonies":
                    var t = await _context.Testimonies.FindAsync(recordId);
                    if (t == null) return false;
                    t.ImageUrl = newUrl;
                    break;

                case "PostModal":
                    var m = await _context.PostModal.FindAsync(recordId);
                    if (m == null) return false;
                    m.ImageUrl = newUrl;
                    break;

                case "SuperSettings":
                    var ss = await _context.SuperSettings.FindAsync(recordId);
                    if (ss == null) return false;
                    if (columnPair == "CompanyLogoKey/Url") { ss.CompanyLogoUrl = newUrl; if (string.IsNullOrWhiteSpace(ss.CompanyLogoKey)) ss.CompanyLogoKey = key; }
                    else if (columnPair == "CompanyWhiteLogoKey/Url") ss.CompanyWhiteLogoUrl = newUrl;
                    else if (columnPair == "CompanyIconKey/Url") ss.CompanyIconUrl = newUrl;
                    else if (columnPair == "LoginBackgroundImageKey/Url") ss.LoginBackgroundImageUrl = newUrl;
                    else if (columnPair == "ReloaderIconKey/Url") ss.ReloaderIconUrl = newUrl;
                    else if (columnPair == "PlayStoreImageKey/Url") ss.PlayStoreImageUrl = newUrl;
                    else if (columnPair == "AppStoreImageKey/Url") ss.AppStoreImageUrl = newUrl;
                    else if (columnPair == "PortfolioBreacrumImageKey/Url") ss.PortfolioBreacrumImageUrl = newUrl;
                    else if (columnPair == "PortfolioImageOneKey/Url") ss.PortfolioImageOneUrl = newUrl;
                    else if (columnPair == "PortfolioImageTwoKey/Url") ss.PortfolioImageTwoUrl = newUrl;
                    else return false;
                    break;

                case "Settings":
                    var st = await _context.Settings.FindAsync((int)recordId);
                    if (st == null) return false;
                    if (columnPair == "DefaultTitleBackgroundKey/Url") st.DefaultTitleBackgroundUrl = newUrl;
                    else if (columnPair == "JobCardTitleBackgroundKey/Url") st.JobCardTitleBackgroundUrl = newUrl;
                    else if (columnPair == "SignatureKey/Url") st.SignatureUrl = newUrl;
                    else return false;
                    break;

                default:
                    return false;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
