using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Services.WhatsApp
{
    public interface IWhatsAppCloudApiService
    {
        Task<bool> SendTextMessageAsync(string toPhoneNumber, string message);
        Task<bool> SendQuickReplyButtonsAsync(string toPhoneNumber, string headerText, string bodyText, List<(string Id, string Title)> buttons, string? footerText = null);
        Task<bool> SendListMenuAsync(string toPhoneNumber, string headerText, string bodyText, string buttonText, List<(string Id, string Title, string Description)> sections, string? footerText = null);
    }
}
