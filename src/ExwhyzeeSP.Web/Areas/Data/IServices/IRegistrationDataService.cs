using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IRegistrationDataService
    {
        Task Create(StudentData model, string pinNumber);
        Task<StudentDataDto> Get(int? id);
        Task<StudentData> GetEdit(int? id);
        Task Edit(StudentData model);
        Task Delete(int? id);
        Task AdmitStudent(int id);
        Task<List<StudentData>> List();
        Task<PinCodeModel> GetPin();
        Task<StudentData> FormData();
        Task<StudentData> FormEmail();
    }
}


