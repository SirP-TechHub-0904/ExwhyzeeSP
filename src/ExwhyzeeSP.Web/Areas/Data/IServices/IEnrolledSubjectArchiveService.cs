using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Core.Entities.ResultArchive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IEnrolledSubjectArchiveService
    {
        Task Create(EnrolledSubjectArchive model);
        Task<EnrolledSubjectArchive> Get(int? id);
        Task Edit(EnrolledSubjectArchive models);
        Task Delete(int? id);
        Task<List<EnrolledSubjectArchive>> List();
        Task EditScore(int id);
    }
}


