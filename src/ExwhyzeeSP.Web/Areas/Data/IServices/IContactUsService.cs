using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IContactUsService
    {
        Task Create(ContactUs model);
        Task<ContactUs> Get(int? id);
        
        Task Delete(int? id);
        Task<List<ContactUs>> List();
        Task Reply(MessageReply model, int id);
       
    }
}


