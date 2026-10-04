using ExwhyzeeSP.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
// // using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Entities;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Dtos;
using Microsoft.AspNetCore.Identity;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class PinService : IPinService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

      

        public async Task<PinInfoDto> Details(int? id)
        {
            var item = await db.PinCodeModels.FirstOrDefaultAsync(x => x.Id == id);
            if(item.EnrollmentId == null && item.StudentPin != null)
            {
                var student = await db.StudentProfiles.Include(j => j.user).FirstOrDefaultAsync(o => o.StudentRegNumber == item.StudentPin);
                var output = new PinInfoDto
                {
                    PinNumber = item.PinNumber,
                    SerialNumber = item.SerialNumber,
                    BatchNumber = item.BatchNumber,
                    Usage = item.Usage,
                    SessionUsed = "NONE",
                    StudentFullName = student.user.Surname + " " + student.user.FirstName + " " + student.user.OtherName,
                    //StudentFullName = "NONE",
                    StudentPin = item.StudentPin,
                  
                };
                return output;
            }
            else if (item.StudentPin != null)
            {
                var enrolment = await db.Enrollments.Include(c => c.Session).Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.Id == item.EnrollmentId);
                var session = await db.Sessions.FirstOrDefaultAsync(c => c.Id == enrolment.SessionId);
                var student = await db.StudentProfiles.Include(j => j.user).FirstOrDefaultAsync(o => o.StudentRegNumber == item.StudentPin && o.Id == enrolment.StudentProfileId);
                var output = new PinInfoDto
                {
                    PinNumber = item.PinNumber,
                    SerialNumber = item.SerialNumber,
                    BatchNumber = item.BatchNumber,
                    Usage = item.Usage,
                    SessionUsed = session.SessionYear + " " + session.Term,
                    StudentFullName = student.user.Surname + " " + student.user.FirstName + " " + student.user.OtherName,
                    StudentPin = item.StudentPin,
                    EnrollmentId = enrolment.Id
                };
                return output;
            }
            else
            {
                var output = new PinInfoDto
                {
                    PinNumber = item.PinNumber,
                    SerialNumber = item.SerialNumber,
                    BatchNumber = item.BatchNumber,
                    Usage = item.Usage,
                    SessionUsed = "NONE",
                    StudentFullName = "NONE",
                    StudentPin = "NONE"
                };
                return output;
            }



        }

        public async Task<List<PinCodeModel>> List(string searchString, string currentFilter, int? page)
        {
            var pins = db.PinCodeModels.Include(x => x.Session).OrderByDescending(x => x.PinNumber).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                var s = searchString.ToUpper();
                pins = pins.Where(p => (p.StudentPin != null && p.StudentPin.ToUpper().Contains(s))
                    || (p.PinNumber != null && p.PinNumber.ToUpper().Contains(s))
                    || (p.SerialNumber != null && p.SerialNumber.ToUpper().Contains(s))
                    || (p.BatchNumber != null && p.BatchNumber.ToUpper().Contains(s)));
            }
            return await pins.ToListAsync();
        }

        public async Task<int> TotalPin()
        {
            var pin = await db.PinCodeModels.CountAsync();
            return pin;
        }

        public async Task<int> TotalUnUsedPin()
        {
            var pin = await db.PinCodeModels.Include(x => x.Session).Where(x => x.StudentPin == null).CountAsync();
            return pin;
        }

        public async Task<int> TotalUsedPin()
        {
            var pin = await db.PinCodeModels.Include(x => x.Session).Where(x => x.StudentPin != null).CountAsync();
            return pin;
        }

        public async Task<List<PinCodeModel>> UnUsedPin(string searchString, string currentFilter, int? page)
        {
            var pins = db.PinCodeModels.Include(x => x.Session).Where(x => x.StudentPin == null).OrderByDescending(x => x.PinNumber).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                var s = searchString.ToUpper();
                pins = pins.Where(p => (p.PinNumber != null && p.PinNumber.ToUpper().Contains(s))
                    || (p.SerialNumber != null && p.SerialNumber.ToUpper().Contains(s))
                    || (p.BatchNumber != null && p.BatchNumber.ToUpper().Contains(s)));
            }
            return await pins.ToListAsync();
        }

        public async Task<List<PinCodeModel>> UsedPin(string searchString, string searchStringSession, string currentFilter, int? page)
        {
            var pins = db.PinCodeModels.Include(x => x.Session).Where(x => x.StudentPin != null).OrderByDescending(x => x.PinNumber).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                var s = searchString.ToUpper();
                pins = pins.Where(p => (p.StudentPin != null && p.StudentPin.ToUpper().Contains(s))
                    || (p.PinNumber != null && p.PinNumber.ToUpper().Contains(s))
                    || (p.SerialNumber != null && p.SerialNumber.ToUpper().Contains(s))
                    || (p.BatchNumber != null && p.BatchNumber.ToUpper().Contains(s)));
            }
            if (!String.IsNullOrEmpty(searchStringSession))
            {
                pins = pins.Where(p => p.SessionId != null && p.SessionId.ToString().Contains(searchStringSession));
            }
            return await pins.ToListAsync();
        }
    }
}

