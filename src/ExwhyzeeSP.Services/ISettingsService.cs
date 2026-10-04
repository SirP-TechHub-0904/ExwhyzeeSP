using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Services
{
    public interface ISettingsService
    {
        Task<VerificationWebDto> ValidateWeb(HttpContext httpContext);
        Task<SuperSetting> GetSuperSettings();
        Task<bool> UseHttps();
        Task<bool> UseHttpsWWW();
    }
}
