using ExwhyzeeSP.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Entities;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class ImageService : IImageService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ImageService()
        {
        }

        public ImageService(
            ApplicationDbContext context = null,
            UserManager<ApplicationUser> userManager = null,
            RoleManager<IdentityRole> roleManager = null)
        {
            if (context != null) db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public UserManager<ApplicationUser> UserManager => _userManager ?? HttpHelper.Current?.RequestServices?.GetService<UserManager<ApplicationUser>>();
        public RoleManager<IdentityRole> RoleManager => _roleManager ?? HttpHelper.Current?.RequestServices?.GetService<RoleManager<IdentityRole>>();

        public async Task<int> Create(IFormFile upload)
        {
            ImageModel model = new ImageModel();
            if (upload != null && upload.Length > 0)
            {
                using var ms = new MemoryStream();
                await upload.CopyToAsync(ms);
                model.ImageContent = ms.ToArray();
                model.ContentType = upload.ContentType;
                model.FileName = upload.FileName;
            }

            db.ImageModel.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpHelper.Current?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null && UserManager != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user?.UserName ?? "";
                    tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Added an image";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }

            return model.Id;
        }

        public async Task Delete(int? id)
        {
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == id);
            if (img != null)
            {
                db.ImageModel.Remove(img);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId = HttpHelper.Current?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null && UserManager != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    if (user != null)
                    {
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId;
                        tracker.UserName = user?.UserName ?? "";
                        tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "deleted an image";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task Edit(int id, IFormFile upload)
        {
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == id);
            if (img != null)
            {
                if (upload != null && upload.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await upload.CopyToAsync(ms);
                    img.ImageContent = ms.ToArray();
                    img.ContentType = upload.ContentType;
                    img.FileName = upload.FileName;
                }
                db.Entry(img).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId = HttpHelper.Current?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null && UserManager != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    if (user != null)
                    {
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId;
                        tracker.UserName = user?.UserName ?? "";
                        tracker.FullName = (user?.Surname ?? "") + " " + (user?.FirstName ?? "") + " " + (user?.OtherName ?? "");
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "edited an image";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<ImageModel> Get(int? id)
        {
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == id);
            return img;
        }

        public async Task PostImageCreate(List<IFormFile> upload, int PostId)
        {
            if (upload != null && upload.Count > 0)
            {
                foreach (var image in upload)
                {
                    PostImage model = new PostImage();
                    using var ms = new MemoryStream();
                    await image.CopyToAsync(ms);
                    model.ImageContent = ms.ToArray();
                    string b4 = Convert.ToBase64String(model.ImageContent);
                    model.ImageByte = "data:image/jpg;base64," + b4;
                    model.ContentType = image.ContentType;
                    model.FileName = image.FileName;
                    model.PostId = PostId;

                    db.PostImages.Add(model);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task PostImageDelete(int? id)
        {
            var img = await db.PostImages.FirstOrDefaultAsync(x => x.Id == id);
            if (img != null)
            {
                db.PostImages.Remove(img);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<PostImage>> PostImageGet(int? PostId)
        {
            var img = await db.PostImages.Where(x => x.PostId == PostId).ToListAsync();
            return img;
        }
    }
}
