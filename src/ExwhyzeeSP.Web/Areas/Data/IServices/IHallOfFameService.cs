using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IHallOfFameService
    {
        Task Create(HallOfFame model, IFormFile upload);
        Task<HallOfFame> Get(int? id);

        Task Edit(HallOfFame models, IFormFile upload);
        Task Delete(int? id);
        Task<List<HallOfFame>> List();
    }
}


