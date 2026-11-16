using ExwhyzeeSP.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.WebSetting
{
     [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,Manager,Admin,Editor")]
    [RequestSizeLimit(104857600)]
    [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
    public class UpdateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;

        public UpdateModel(ApplicationDbContext context, IConfiguration config, IStorageService storageService)
        {
            _context = context;
            _config = config;
            _storageService = storageService;
        }
        [BindProperty]
        public IFormFile? appStoreImage { get; set; }
        [BindProperty]
        public IFormFile? playStoreImage { get; set; }
        [BindProperty]
        public IFormFile? reloaderimage { get; set; }
        [BindProperty]
        public IFormFile? imagefilewhitecompany { get; set; }

        [BindProperty]
        public IFormFile? imagefilecompany { get; set; }
        [BindProperty]
        public IFormFile? imagefilecompanyicon { get; set; }
        [BindProperty]
        public IFormFile? imagetitlebg { get; set; }
        [BindProperty]
        public Setting Setting { get; set; }
        [BindProperty]
        public SuperSetting SuperSetting { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
             
            Setting = await _context.Settings.FirstOrDefaultAsync();

            if (Setting == null)
            {
                return NotFound();
            }
            SuperSetting = await _context.SuperSettings.FirstOrDefaultAsync();

            if (SuperSetting == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var updatesetting = await _context.Settings.FirstOrDefaultAsync();
            var superupdate = await _context.SuperSettings.FirstOrDefaultAsync();
            //image
            if (imagefilecompany != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await imagefilecompany.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(imagefilecompany.FileName);
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

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, superupdate.CompanyLogoKey);
                    // 
                    if (!string.IsNullOrEmpty(xresult.Url) || (xresult.Message != null && (xresult.Message.Contains("200") || xresult.Message.Equals("Success", StringComparison.OrdinalIgnoreCase))))
                    {
                        superupdate.CompanyLogoUrl = xresult.Url;
                        superupdate.CompanyLogoKey = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload image: " + xresult.Message;
                    }
                }
                catch (Exception c)
                {
                    TempData["error"] = "unable to upload image: " + c.Message;
                }
            }

            if (imagefilewhitecompany != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await imagefilewhitecompany.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(imagefilewhitecompany.FileName);
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

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, superupdate.CompanyWhiteLogoKey);
                    // 
                    if (!string.IsNullOrEmpty(xresult.Url) || (xresult.Message != null && (xresult.Message.Contains("200") || xresult.Message.Equals("Success", StringComparison.OrdinalIgnoreCase))))
                    {
                        superupdate.CompanyWhiteLogoUrl = xresult.Url;
                        superupdate.CompanyWhiteLogoKey = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload white logo: " + xresult.Message;
                    }
                }
                catch (Exception c)
                {
                    TempData["error"] = "unable to upload white logo: " + c.Message;
                }
            }

            if (imagefilecompanyicon != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await imagefilecompanyicon.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(imagefilecompanyicon.FileName);
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

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, superupdate.CompanyIconKey);
                    // 
                    if (!string.IsNullOrEmpty(xresult.Url) || (xresult.Message != null && (xresult.Message.Contains("200") || xresult.Message.Equals("Success", StringComparison.OrdinalIgnoreCase))))
                    {
                        superupdate.CompanyIconUrl = xresult.Url;
                        superupdate.CompanyIconKey = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload favicon/icon: " + xresult.Message;
                    }
                }
                catch (Exception c)
                {
                    TempData["error"] = "unable to upload favicon/icon: " + c.Message;
                }
            }
            if (imagetitlebg != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await imagetitlebg.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(imagetitlebg.FileName);
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

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, updatesetting.DefaultTitleBackgroundKey);
                    // 
                    if (!string.IsNullOrEmpty(xresult.Url) || (xresult.Message != null && (xresult.Message.Contains("200") || xresult.Message.Equals("Success", StringComparison.OrdinalIgnoreCase))))
                    {
                        updatesetting.DefaultTitleBackgroundUrl = xresult.Url;
                        updatesetting.DefaultTitleBackgroundKey = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload background image: " + xresult.Message;
                    }
                }
                catch (Exception c)
                {
                    TempData["error"] = "unable to upload background image: " + c.Message;
                }
            }
            ////
            ///
            if (reloaderimage != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await reloaderimage.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(reloaderimage.FileName);
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

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, superupdate.ReloaderIconKey);
                    // 
                    if (!string.IsNullOrEmpty(xresult.Url) || (xresult.Message != null && (xresult.Message.Contains("200") || xresult.Message.Equals("Success", StringComparison.OrdinalIgnoreCase))))
                    {
                        superupdate.ReloaderIconUrl = xresult.Url;
                        superupdate.ReloaderIconKey = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload reloader icon: " + xresult.Message;
                    }
                }
                catch (Exception c)
                {
                    TempData["error"] = "unable to upload reloader icon: " + c.Message;
                }
            }
            if (appStoreImage != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await appStoreImage.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(appStoreImage.FileName);
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

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, superupdate.AppStoreImageKey);
                    // 
                    if (!string.IsNullOrEmpty(xresult.Url) || (xresult.Message != null && (xresult.Message.Contains("200") || xresult.Message.Equals("Success", StringComparison.OrdinalIgnoreCase))))
                    {
                        superupdate.AppStoreImageUrl = xresult.Url;
                        superupdate.AppStoreImageKey = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload App Store badge: " + xresult.Message;
                    }
                }
                catch (Exception c)
                {
                    TempData["error"] = "unable to upload App Store badge: " + c.Message;
                }
            }

            if (playStoreImage != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await playStoreImage.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(playStoreImage.FileName);
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

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, superupdate.PlayStoreImageKey);
                    // 
                    if (!string.IsNullOrEmpty(xresult.Url) || (xresult.Message != null && (xresult.Message.Contains("200") || xresult.Message.Equals("Success", StringComparison.OrdinalIgnoreCase))))
                    {
                        superupdate.PlayStoreImageUrl = xresult.Url;
                        superupdate.PlayStoreImageKey = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload Play Store badge: " + xresult.Message;
                    }
                }
                catch (Exception c)
                {
                    TempData["error"] = "unable to upload Play Store badge: " + c.Message;
                }
            }

            ///
            superupdate.CompanyName = SuperSetting.CompanyName;
            superupdate.CompanyDescription = SuperSetting.CompanyDescription;
            superupdate.CompanyWebsiteLink = SuperSetting.CompanyWebsiteLink;
            superupdate.WebsiteTitle = SuperSetting.WebsiteTitle;
            superupdate.ProductTitle = SuperSetting.ProductTitle;
            superupdate.ProductTitleHome = SuperSetting.ProductTitleHome;
            superupdate.SpecialMenuButtonActivate = SuperSetting.SpecialMenuButtonActivate;
            superupdate.SpecialMenuButtonLink = SuperSetting.SpecialMenuButtonLink;
            superupdate.SpecialMenuButtonText = SuperSetting.SpecialMenuButtonText;


            superupdate.ActivateWorkWithUsFooter = SuperSetting.ActivateWorkWithUsFooter;
            superupdate.ActivateDownloadOurAppFooter = SuperSetting.ActivateDownloadOurAppFooter;
            superupdate.TitleDownloadOurAppFooter = SuperSetting.TitleDownloadOurAppFooter;
            superupdate.DescriptionDownloadOurAppFooter = SuperSetting.DescriptionDownloadOurAppFooter;
            superupdate.AppStoreLink = SuperSetting.AppStoreLink;
            superupdate.PlayStoreLink = SuperSetting.PlayStoreLink;
            superupdate.ActivateSubscribeFormOnFooter = SuperSetting.ActivateSubscribeFormOnFooter; 
 

            updatesetting.Qoute = Setting.Qoute;
            updatesetting.TopNote = Setting.TopNote;
            updatesetting.FacebookPage = Setting.FacebookPage;
            updatesetting.InstagramPage = Setting.InstagramPage;
            updatesetting.TwitterPage = Setting.TwitterPage;
            updatesetting.TiktokPage = Setting.TiktokPage;
            updatesetting.YoutubeChannel = Setting.YoutubeChannel;
            updatesetting.WorkingHour = Setting.WorkingHour;
            updatesetting.AddressOne = Setting.AddressOne;
            updatesetting.AddressTwo = Setting.AddressTwo;
            updatesetting.EmailOne = Setting.EmailOne;
            updatesetting.ShowEmailOneInTop = Setting.ShowEmailOneInTop;
            updatesetting.ShowEmailOneInFooter = Setting.ShowEmailOneInFooter;
            updatesetting.EmailTwo = Setting.EmailTwo;
            updatesetting.ShowEmailTwoInTop = Setting.ShowEmailTwoInTop;
            updatesetting.ShowEmailTwoInFooter = Setting.ShowEmailTwoInFooter;
            updatesetting.EmailThree = Setting.EmailThree;
            updatesetting.ShowEmailThreeInTop = Setting.ShowEmailThreeInTop;
            updatesetting.ShowEmailThreeInFooter = Setting.ShowEmailThreeInFooter;
            updatesetting.PhoneOne = Setting.PhoneOne;
            updatesetting.ShowPhoneOneInTop = Setting.ShowPhoneOneInTop;
            updatesetting.ShowPhoneOneInFooter = Setting.ShowPhoneOneInFooter;
            updatesetting.PhoneTwo = Setting.PhoneTwo;
            updatesetting.ShowPhoneTwoInTop = Setting.ShowPhoneTwoInTop;
            updatesetting.ShowPhoneTwoInFooter = Setting.ShowPhoneTwoInFooter;
            updatesetting.PhoneThree = Setting.PhoneThree;
            updatesetting.ShowPhoneThreeInTop = Setting.ShowPhoneThreeInTop;
            updatesetting.ShowPhoneThreeInFooter = Setting.ShowPhoneThreeInFooter;
            updatesetting.GoogleMap = Setting.GoogleMap;
            updatesetting.ShowContactUsMenu = Setting.ShowContactUsMenu;
            updatesetting.ShowContactUsFooter = Setting.ShowContactUsFooter;
            updatesetting.ShowAddressOneInTop = Setting.ShowAddressOneInTop;
            updatesetting.AddBlogToFooter = Setting.AddBlogToFooter;
            updatesetting.AddBlogToHome = Setting.AddBlogToHome;
            updatesetting.AddBlogToMenu = Setting.AddBlogToMenu;
            updatesetting.BlogDisplayTitle = Setting.BlogDisplayTitle;

               updatesetting.AddCareerToFooter = Setting.AddCareerToFooter; 
            updatesetting.AddCareerToMenu = Setting.AddCareerToMenu;
            updatesetting.CareerDisplayTitle = Setting.CareerDisplayTitle;


            updatesetting.AddFaqToHome = Setting.AddFaqToHome;
            updatesetting.AddFaqToFooter = Setting.AddFaqToFooter;
            updatesetting.AddTestimonyToHome = Setting.AddTestimonyToHome;
            updatesetting.AddTestimonyToFooter = Setting.AddTestimonyToFooter;
            updatesetting.DisableMainTopMenu = Setting.DisableMainTopMenu;
            updatesetting.ShowProductsInMenu = Setting.ShowProductsInMenu;
            updatesetting.ShowProductsInFooter = Setting.ShowProductsInFooter;
            updatesetting.ActivateProductsInHome = Setting.ActivateProductsInHome;
            updatesetting.ShowTwoProducts = Setting.ShowTwoProducts;
            updatesetting.ShowThreeProducts = Setting.ShowThreeProducts;
            updatesetting.ShowSixProducts = Setting.ShowSixProducts; 
            updatesetting.EnableBreakingNewsRibon = Setting.EnableBreakingNewsRibon; 
            updatesetting.RibonPosition = Setting.RibonPosition;
            updatesetting.BreakingNewsRibonTitle = Setting.BreakingNewsRibonTitle;
            try
            {
                await _context.SaveChangesAsync();
                TempData["success"] = "Website settings updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error saving settings: " + ex.Message;
            }
           

            return RedirectToPage("./Index");
        }

         
    }
}





