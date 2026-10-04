using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Web.Areas.Data.Services;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Entities;

namespace ExwhyzeeSP.Web.Areas.Financial.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finance")]
    [Area("Financial")]
    public class BankStatementController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IBankStatementService _bankStatementService = new BankStatementService();

        public BankStatementController(BankStatementService bankStatementService)
        {
            _bankStatementService = bankStatementService;
        }

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public BankStatementController()
        {

        }
        // GET: Financial/BankStatement
        public async Task<ActionResult> Index()
        {
            var item = await _bankStatementService.BankStatementList();
            return View(item);
        }

        public ActionResult CreateStatement()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateStatement(BankDetails bank)
        {
            if (ModelState.IsValid)
            {
                await _bankStatementService.Create(bank);
                TempData["success"] = "Bank Statement Created Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to create Bank Statement";
            return RedirectToAction("Index");
        }
    }
}

