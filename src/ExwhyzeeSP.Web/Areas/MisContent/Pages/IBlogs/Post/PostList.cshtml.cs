using ExwhyzeeSP.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.IBlogs.Post
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin,Editor")]

    public class PostListModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PostListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Blog> Blog { get; set; }

        public async Task OnGetAsync(long id)
        {
            Blog = await _context.Blogs
                .Include(b => b.BlogCategory).Where(x=>x.BlogCategoryId == id).ToListAsync();
        }
    }
}



