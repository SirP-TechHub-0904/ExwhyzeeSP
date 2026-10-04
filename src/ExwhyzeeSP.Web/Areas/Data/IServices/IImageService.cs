using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IImageService
    {
        Task<int> Create(IFormFile upload);
        Task<ImageModel> Get(int? id);
        Task Edit(int id, IFormFile upload);
        Task Delete(int? id);

        Task PostImageCreate(List<IFormFile> upload, int PostId);
        Task<List<PostImage>> PostImageGet(int? PostId);
        Task PostImageDelete(int? PostId);

    }
}


