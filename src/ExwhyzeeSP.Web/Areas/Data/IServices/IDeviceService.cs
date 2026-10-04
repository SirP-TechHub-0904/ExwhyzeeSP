using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface IDeviceService
    {
        Task Create(ApprovedDevice model);
        Task Edit(ApprovedDevice model);
        Task<List<ApprovedDevice>> DeviceList();
        Task Delete(int? id);
        Task<ApprovedDevice> Get(int? id);
    }
}


