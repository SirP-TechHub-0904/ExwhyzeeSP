using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface INotificationSettingService
    {
        Task Create(Notification model);
        Task<Notification> Get(int? id);
        Task<NotificationDto> GetNotification();
        Task Edit(Notification models);
    }
}


