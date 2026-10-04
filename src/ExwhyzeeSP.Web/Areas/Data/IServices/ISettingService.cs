using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.IServices
{
    interface ISettingService
    {
        Task Create(Setting model);
        Task<Setting> Get(int? id);
        Task<SettingsDto> GetSetting();
        Task Edit(Setting models);


        Task CreateSettingValue(SettingsValue model);
        Task<SettingsValue> GetSettingValue(int? id);
        Task DeleteSettingValue(int? id);
        Task EditSettingValue(SettingsValue models);

        Task<List<SettingsValue>> SettingsValueList();
    }
}


