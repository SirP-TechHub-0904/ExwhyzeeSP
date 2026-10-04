
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Pages.Shared.ViewComponents
{
    public class ModalViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ModalViewComponent(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
             var modal = await _context.PostModal.FirstOrDefaultAsync(x=>x.Publish == true);
             ViewBag.modal = modal;
            return View();
        }
    }
}


