using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IPostService
    {
        Task<int> Create(Post model);
        Task<Post> Get(int? id);
        Task Edit(Post models);
        Task Delete(int? id);
       
        Task<List<Post>> List(string searchString, string currentFilter, int? page);
        Task<List<Post>> StaffPost(string searchString, string currentFilter, int? page);
        Task<List<Post>> StudentPost(string searchString, string currentFilter, int? page);

        Task<Post> AdminDetails(int? id);
        Task<Post> Details(int? id);
        Task<List<Post>> ListPost(int count);


        Task CreateComment(Comment model);
        Task<Comment> GetComment(int? id);
       
        Task DeleteComment(int? id);

    }
}


