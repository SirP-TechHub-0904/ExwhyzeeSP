using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IStudentProfileService
    {
        Task<StudentInfoDto> Get(int? id);
        Task UpdateImageId(int id, int imgId);
        Task Edit(StudentInfoDto models);
        
        Task<List<StudentProfile>> List();

        Task<string> StudentCurrentClass(int? profileId);

        Task<StudentInfoDto> GetStudentWithoutId();
        Task<StudentProfile> GetStudentByUserId(string id);
        Task<int> SubjectCountStudent();
        Task<string> ClassNameForStudent();
    }
}


