using System;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Services
{
    public class EmailLoggingService : IEmailLoggingService
    {
        private readonly MasterDbContext _context;

        public EmailLoggingService(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<MasterEmailLog> LogAndDumpEmailAsync(
            string recipientEmail,
            string subject,
            string bodyHtml,
            string? recipientName = null,
            string? bodyText = null,
            string category = "Notification",
            string? triggeredBy = null,
            string? tenantSubDomain = null,
            string? senderEmail = "noreply@exwhyzee.ng")
        {
            var emailLog = new MasterEmailLog
            {
                CreatedAtUtc = DateTime.UtcNow,
                RecipientEmail = recipientEmail,
                RecipientName = recipientName,
                SenderEmail = senderEmail ?? "noreply@exwhyzee.ng",
                Subject = subject,
                BodyHtml = bodyHtml,
                BodyText = bodyText,
                EmailCategory = category,
                Status = "LoggedAndDumped",
                TriggeredBy = triggeredBy ?? "System",
                TenantSubDomain = tenantSubDomain
            };

            try
            {
                await _context.EmailLogs.AddAsync(emailLog);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Safety guard: failure to write email log should not interrupt parent business logic
            }

            return emailLog;
        }
    }
}
