using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;

namespace ExwhyzeeSP.Web.Controllers
{
    public class RegistrationController : Controller
    {
            private ApplicationDbContext db = new ApplicationDbContext();
           
            // GET: Registration
            public ActionResult Index()
        {
            var sett = db.Settings.FirstOrDefault();
            if (sett.AdmissionPinOption == AdmissionPinOption.UsedPin)
            {
                return RedirectToAction("Index","Apply",new { area= "Admission"});
            }
            else if (sett.AdmissionPinOption == AdmissionPinOption.NoPin)
            {
                return RedirectToAction("Index2", "Apply", new { area = "Admission" });


            }
            return View();
        }

    }
}

