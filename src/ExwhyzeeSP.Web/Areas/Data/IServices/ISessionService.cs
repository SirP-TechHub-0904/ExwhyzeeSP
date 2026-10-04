using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface ISessionService
    {
        Task Create(Session model);
        Task<bool> NextSession();
        Task<bool> PreviousSession();
        Task<Session> Get(int? id);
        Task<SchoolSessionDto> GetInfo(int id);
        Task Edit(Session models);
        Task Delete(int? id);
        Task<List<SchoolSessionDto>> GetAllSession();
        Task<List<Session>> List();

        Task<int> GetCurrentSessionId();

        Task<string> GetCurrentSession();

        Task<string> GetCurrentSessionTerm();

    }
}


