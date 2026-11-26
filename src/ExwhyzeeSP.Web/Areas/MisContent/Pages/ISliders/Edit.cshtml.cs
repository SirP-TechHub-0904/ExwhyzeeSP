using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.ISliders
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,Editor")]

    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;

        public EditModel(ApplicationDbContext context, IConfiguration config, IStorageService storageService)
        {
            _context = context;
            _config = config;
            _storageService = storageService;
        }


        [BindProperty]
        public IFormFile? imagefile { get; set; }

        [BindProperty]
        public IFormFile? imagefile2 { get; set; }
        [BindProperty]
        public IFormFile? videofile { get; set; }


        [BindProperty]
        public Slider Slider { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

            if (Slider == null)
            {
                return NotFound();
            }

            ViewData["WebPageId"] = new SelectList(_context.WebPages, "Id", "Title");


            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //image
            if (imagefile != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await imagefile.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(imagefile.FileName);
                    var docName = $"{Guid.NewGuid()}{fileExt}";
                    // call server

                    var s3Obj = new S3Object()
                    {
                        BucketName = await _storageService.BucketName(),
                        InputStream = memoryStream,
                        Name = docName
                    };

                    var cred = new AwsCredentials()
                    {
                        AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                        SecretKey = _config["AwsConfiguration:AWSSecretKey"]
                    };

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, Slider.Key);
                    // 
                    if (!string.IsNullOrEmpty(xresult.Url) || (xresult.Message != null && (xresult.Message.Contains("200") || xresult.Message.Equals("Success", StringComparison.OrdinalIgnoreCase))))
                    {
                        Slider.Url = xresult.Url;
                        Slider.Key = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload image: " + xresult.Message;
                        //return Page();
                    }
                }
                catch (Exception c)
                {
                    TempData["error"] = "unable to upload image: " + c.Message;
                }
            }

            if (imagefile2 != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await imagefile2.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(imagefile2.FileName);
                    var docName = $"{Guid.NewGuid()}{fileExt}";
                    // call server

                    var s3Obj = new S3Object()
                    {
                        BucketName = await _storageService.BucketName(),
                        InputStream = memoryStream,
                        Name = docName
                    };

                    var cred = new AwsCredentials()
                    {
                        AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                        SecretKey = _config["AwsConfiguration:AWSSecretKey"]
                    };

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, "");
                    // 
                    if (!string.IsNullOrEmpty(xresult.Url) || (xresult.Message != null && (xresult.Message.Contains("200") || xresult.Message.Equals("Success", StringComparison.OrdinalIgnoreCase))))
                    {
                        Slider.SecondUrl = xresult.Url;
                        Slider.SecondKey = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload image: " + xresult.Message;
                        //return Page();
                    }
                }
                catch (Exception c)
                {
                    TempData["error"] = "unable to upload image: " + c.Message;
                }
            }
            _context.Attach(Slider).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); TempData["success"] = "Successful";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SliderExists(Slider.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SliderExists(long id)
        {
            ViewData["WebPageId"] = new SelectList(_context.WebPages, "Id", "Title");

            return _context.Sliders.Any(e => e.Id == id);
        }
    }
}





