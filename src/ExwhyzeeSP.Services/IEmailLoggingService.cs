using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Services
{
    public interface IEmailLoggingService
    {
        Task<MasterEmailLog> LogAndDumpEmailAsync(
            string recipientEmail,
            string subject,
            string bodyHtml,
            string? recipientName = null,
            string? bodyText = null,
            string category = "Notification",
            string? triggeredBy = null,
            string? tenantSubDomain = null,
            string? senderEmail = "noreply@exwhyzee.ng");
    }
}
