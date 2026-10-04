using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IPinService
    {
        Task<List<PinCodeModel>> List(string searchString, string currentFilter, int? page);

        Task<List<PinCodeModel>> UsedPin(string searchString, string searchStringSession, string currentFilter, int? page);

        Task<List<PinCodeModel>> UnUsedPin(string searchString, string currentFilter, int? page);

        Task<PinInfoDto> Details(int? id);

        Task<int> TotalPin();
        Task<int> TotalUsedPin();
        Task<int> TotalUnUsedPin();
    }
}


