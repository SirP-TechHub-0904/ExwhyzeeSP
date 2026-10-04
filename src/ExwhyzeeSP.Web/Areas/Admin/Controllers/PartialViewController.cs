using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExwhyzeeSP.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PartialViewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/PartialView
        public ActionResult LayoutProfile()
        {
            var item = db.Settings.FirstOrDefault();

            var img =  db.ImageModel.FirstOrDefault(x => x.Id == item.ImageId);
            var output = new SettingLayoutDto
            {
                Id = item.Id,
               SchoolName = item.SchoolName,
                SchoolInitials = item.SchoolInitials,
                
                ContactEmail = item.ContactEmail,
                Image = img.ImageContent,
               
            };

            return PartialView(output);
        }

        public ActionResult LayoutProfileTitle()
        {
            var item = db.Settings.FirstOrDefault();
            return PartialView(item);
        }

        public ActionResult LayoutSchoolName()
        {
            var item = db.Settings.FirstOrDefault().SchoolName;

            return PartialView(item);
        }

        public ActionResult SchoolIcon()
        {
            var item = db.Settings.FirstOrDefault();

            var img = db.ImageModel.FirstOrDefault(x => x.Id == item.ImageId);
            var output = new SettingLayoutDto
            {
               
                Image = img.ImageContent

            };

            return PartialView(output);
        }
    }
}

