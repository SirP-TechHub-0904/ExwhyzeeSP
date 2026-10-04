using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IimageSliderServices
    {
        Task New(ImageSlider models, IFormFile upload);
        Task Delete(int? id);
        Task<List<ImageSlider>> List();
        Task AddToSlider(ImageSlider models);
    }
}


