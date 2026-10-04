using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExwhyzeeSP.Web.Areas.Financial.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finance")]

    [Area("Financial")]
    public class FinanceAccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Financial/Account
        public async Task<ActionResult> Complete(int sessionId = 0)
        {
            if (sessionId == 0)
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sessionId = currentSession.Id;
            }


            IQueryable<Finance> Payment = from s in db.Finances
                                             .OrderByDescending(x => x.Date)
                                             .Where(x => x.SessionId == sessionId)
                                             .Where(x => x.TransactionStatus == ExwhyzeeSP.Core.Entities.TransactionStatus.Paid)
                                          select s;

            return View(Payment);
        }

        public async Task<ActionResult> Part(int sessionId = 0)
        {
            if (sessionId == 0)
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sessionId = currentSession.Id;
            }


            IQueryable<Finance> Payment = from s in db.Finances
                                             .OrderByDescending(x => x.Date)
                                             .Where(x => x.SessionId == sessionId)
                                             .Where(x => x.TransactionStatus == ExwhyzeeSP.Core.Entities.TransactionStatus.Part)
                                          select s;

            return View(Payment);
        }

        public async Task<ActionResult> NotPaid(int sessionId = 0)
        {
            if (sessionId == 0)
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sessionId = currentSession.Id;
            }

            IQueryable<Finance> Payment = from s in db.Finances
                                             .OrderByDescending(x => x.Date)
                                             .Where(x => x.SessionId == sessionId)
                                             .Where(x => x.TransactionStatus == ExwhyzeeSP.Core.Entities.TransactionStatus.Part)
                                          select s;

            return View(Payment);
        }
    }
}

