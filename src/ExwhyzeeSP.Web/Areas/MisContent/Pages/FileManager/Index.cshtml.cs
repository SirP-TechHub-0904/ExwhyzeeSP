using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.FileManager
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,SubAdmin,mSuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public IndexModel(ApplicationDbContext context, IStorageService storageService, IWebHostEnvironment env, IConfiguration config)
        {
            _context = context;
            _storageService = storageService;
            _env = env;
            _config = config;
        }

        public IList<PageSection> MediaFiles { get; set; } = new List<PageSection>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public int TotalCount { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.PageSections
                .Where(p => p.TemplateKey == "FileManager" || p.CustomClass == "file-manager-asset");

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(p => (p.Title != null && p.Title.ToLower().Contains(term)) ||
                                         (p.Description != null && p.Description.ToLower().Contains(term)));
            }

            TotalCount = await query.CountAsync();
            MediaFiles = await query
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files, string? customTitle)
        {
            if (files == null || files.Count == 0)
            {
                TempData["error"] = "Please select at least one image file to upload.";
                return RedirectToPage();
            }

            int successCount = 0;
            var localUploadDir = Path.Combine(_env.WebRootPath, "uploads", "filemanager");
            if (!Directory.Exists(localUploadDir))
            {
                Directory.CreateDirectory(localUploadDir);
            }

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var ext = Path.GetExtension(file.FileName).ToLower();
                var originalName = Path.GetFileNameWithoutExtension(file.FileName);
                var title = !string.IsNullOrWhiteSpace(customTitle) && files.Count == 1
                    ? customTitle.Trim()
                    : originalName;

                string imageUrl = "";
                string imageKey = "";

                // Attempt cloud upload via StorageService first
                try
                {
                    var uploadResult = await _storageService.MainUploadFileReturnUrlAsync("", file);
                    if (uploadResult != null && uploadResult.Message.Contains("200") && !string.IsNullOrWhiteSpace(uploadResult.Url))
                    {
                        imageUrl = uploadResult.Url;
                        imageKey = uploadResult.Key ?? "";
                    }
                }
                catch
                {
                    // Fall back to local storage
                }

                // If cloud upload returned empty or failed, save locally
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                    var physicalPath = Path.Combine(localUploadDir, uniqueFileName);
                    await using (var stream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    imageUrl = $"/uploads/filemanager/{uniqueFileName}";
                    imageKey = uniqueFileName;
                }

                // Format file size
                var sizeKb = Math.Round(file.Length / 1024.0, 1);
                var sizeText = sizeKb > 1024 ? $"{Math.Round(sizeKb / 1024.0, 2)} MB" : $"{sizeKb} KB";

                var assetSection = new PageSection
                {
                    Title = title,
                    ImageUrl = imageUrl,
                    ImageKey = imageKey,
                    TemplateKey = "FileManager",
                    CustomClass = "file-manager-asset",
                    Description = $"{sizeText} • {DateTime.Now:dd MMM yyyy, HH:mm}",
                    MiniTitle = ext.Replace(".", "").ToUpper(),
                    ShowInHome = false,
                    Disable = true
                };

                _context.PageSections.Add(assetSection);
                successCount++;
            }

            if (successCount > 0)
            {
                await _context.SaveChangesAsync();
                TempData["success"] = $"Successfully uploaded {successCount} image{(successCount > 1 ? "s" : "")}!";
            }
            else
            {
                TempData["error"] = "Upload failed. Please try again.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var section = await _context.PageSections.FindAsync(id);
            if (section != null)
            {
                // Try deleting from cloud or local storage
                if (!string.IsNullOrWhiteSpace(section.ImageKey))
                {
                    try
                    {
                        var creds = new AwsCredentials
                        {
                            AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                            SecretKey = _config["AwsConfiguration:AWSSecretKey"]
                        };
                        var bucket = await _storageService.BucketName();
                        await _storageService.DeleteObjectAsync(creds, bucket, section.ImageKey);
                    }
                    catch { }

                    // Also check if local file exists
                    try
                    {
                        var localPath = Path.Combine(_env.WebRootPath, "uploads", "filemanager", section.ImageKey);
                        if (System.IO.File.Exists(localPath))
                        {
                            System.IO.File.Delete(localPath);
                        }
                    }
                    catch { }
                }

                _context.PageSections.Remove(section);
                await _context.SaveChangesAsync();
                TempData["success"] = "File deleted successfully!";
            }
            return RedirectToPage();
        }
    }
}
