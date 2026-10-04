using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ExwhyzeeSP.Services
{
    public class MarketingService : IMarketingService
    {
        private readonly MasterDbContext _context;
        private readonly IEmailLoggingService _emailLoggingService;
        private readonly IAuditLogService _auditLogService;

        private static readonly Regex EmailRegex = new(
            @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex ExtractEmailsRegex = new(
            @"[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public MarketingService(
            MasterDbContext context,
            IEmailLoggingService emailLoggingService,
            IAuditLogService auditLogService)
        {
            _context = context;
            _emailLoggingService = emailLoggingService;
            _auditLogService = auditLogService;
        }

        public async Task<MarketingImportResult> ImportContactsFromExcelAsync(Stream fileStream, string defaultTag = "General")
        {
            var result = new MarketingImportResult();
            if (fileStream == null || fileStream.Length == 0)
            {
                result.ErrorMessages.Add("Uploaded spreadsheet is empty.");
                return result;
            }

            try
            {
                using var workbook = new XLWorkbook(fileStream);
                var worksheet = workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.ErrorMessages.Add("No worksheets found in the Excel workbook.");
                    return result;
                }

                var range = worksheet.RangeUsed();
                if (range == null)
                {
                    result.ErrorMessages.Add("Worksheet contains no data.");
                    return result;
                }

                var rows = range.RowsUsed().ToList();
                if (rows.Count == 0)
                {
                    result.ErrorMessages.Add("No rows detected.");
                    return result;
                }

                // Determine column indexes from header row
                var headerRow = rows[0];
                int emailCol = -1, nameCol = -1, phoneCol = -1, schoolCol = -1, roleCol = -1, stateCol = -1, tagCol = -1;

                int colCount = headerRow.CellCount();
                for (int c = 1; c <= colCount; c++)
                {
                    var headerVal = headerRow.Cell(c).GetString()?.Trim().ToLowerInvariant() ?? string.Empty;
                    if (headerVal.Contains("email") || headerVal.Contains("e-mail")) emailCol = c;
                    else if (headerVal.Contains("name") || headerVal.Contains("contact") || headerVal.Contains("proprietor")) nameCol = c;
                    else if (headerVal.Contains("phone") || headerVal.Contains("mobile") || headerVal.Contains("whatsapp") || headerVal.Contains("tel")) phoneCol = c;
                    else if (headerVal.Contains("school") || headerVal.Contains("institution") || headerVal.Contains("academy") || headerVal.Contains("college")) schoolCol = c;
                    else if (headerVal.Contains("role") || headerVal.Contains("title") || headerVal.Contains("position") || headerVal.Contains("designation")) roleCol = c;
                    else if (headerVal.Contains("state") || headerVal.Contains("city") || headerVal.Contains("location")) stateCol = c;
                    else if (headerVal.Contains("tag") || headerVal.Contains("group") || headerVal.Contains("category")) tagCol = c;
                }

                // Fallback if headers were not recognized
                if (emailCol == -1)
                {
                    // Check first row cells for email pattern
                    bool firstRowHasEmail = false;
                    for (int c = 1; c <= colCount; c++)
                    {
                        var val = headerRow.Cell(c).GetString()?.Trim() ?? string.Empty;
                        if (EmailRegex.IsMatch(val))
                        {
                            emailCol = c;
                            firstRowHasEmail = true;
                            break;
                        }
                    }

                    if (emailCol == -1) emailCol = 1; // Default to column 1
                    if (nameCol == -1 && colCount >= 2) nameCol = 2;
                    if (phoneCol == -1 && colCount >= 3) phoneCol = 3;
                    if (schoolCol == -1 && colCount >= 4) schoolCol = 4;
                    if (roleCol == -1 && colCount >= 5) roleCol = 5;
                    if (stateCol == -1 && colCount >= 6) stateCol = 6;
                }

                // Load existing emails for quick in-memory deduplication lookup
                var existingContacts = await _context.MarketingContacts
                    .ToDictionaryAsync(c => c.Email.Trim().ToLowerInvariant(), c => c);

                int startRowIndex = (emailCol != -1 && headerRow.Cell(emailCol).GetString().Contains("@")) ? 0 : 1;

                for (int i = startRowIndex; i < rows.Count; i++)
                {
                    result.TotalProcessed++;
                    var row = rows[i];

                    var rawEmail = emailCol > 0 ? row.Cell(emailCol).GetString()?.Trim() : null;
                    if (string.IsNullOrWhiteSpace(rawEmail) || !EmailRegex.IsMatch(rawEmail))
                    {
                        result.InvalidCount++;
                        continue;
                    }

                    var cleanEmail = rawEmail.Trim().ToLowerInvariant();
                    var name = nameCol > 0 ? row.Cell(nameCol).GetString()?.Trim() : null;
                    var phone = phoneCol > 0 ? row.Cell(phoneCol).GetString()?.Trim() : null;
                    var school = schoolCol > 0 ? row.Cell(schoolCol).GetString()?.Trim() : null;
                    var role = roleCol > 0 ? row.Cell(roleCol).GetString()?.Trim() : null;
                    var state = stateCol > 0 ? row.Cell(stateCol).GetString()?.Trim() : null;
                    var tag = tagCol > 0 ? row.Cell(tagCol).GetString()?.Trim() : null;

                    if (string.IsNullOrWhiteSpace(tag)) tag = defaultTag;

                    if (existingContacts.TryGetValue(cleanEmail, out var existing))
                    {
                        // Update existing contact metadata if new fields are provided
                        if (!string.IsNullOrWhiteSpace(name)) existing.FullName = name;
                        if (!string.IsNullOrWhiteSpace(phone)) existing.PhoneNumber = phone;
                        if (!string.IsNullOrWhiteSpace(school)) existing.SchoolName = school;
                        if (!string.IsNullOrWhiteSpace(role)) existing.RoleTitle = role;
                        if (!string.IsNullOrWhiteSpace(state)) existing.State = state;
                        if (!string.IsNullOrWhiteSpace(tag) && tag != "General") existing.Tag = tag;
                        result.UpdatedCount++;
                    }
                    else
                    {
                        var newContact = new MarketingContact
                        {
                            Email = cleanEmail,
                            FullName = name,
                            PhoneNumber = phone,
                            SchoolName = school,
                            RoleTitle = role,
                            State = state,
                            Tag = tag,
                            Source = "ExcelImport",
                            IsSubscribed = true,
                            CreatedAtUtc = DateTime.UtcNow
                        };
                        _context.MarketingContacts.Add(newContact);
                        existingContacts[cleanEmail] = newContact;
                        result.ImportedCount++;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add($"Error parsing Excel file: {ex.Message}");
            }

            return result;
        }

        public async Task<MarketingImportResult> ImportContactsFromRawTextAsync(
            string rawText, 
            string defaultTag = "General", 
            string? defaultRole = null, 
            string? defaultSchool = null)
        {
            var result = new MarketingImportResult();
            if (string.IsNullOrWhiteSpace(rawText))
            {
                result.ErrorMessages.Add("Pasted content is empty.");
                return result;
            }

            try
            {
                var existingContacts = await _context.MarketingContacts
                    .ToDictionaryAsync(c => c.Email.Trim().ToLowerInvariant(), c => c);

                // Split text into lines or items
                var lines = rawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                    // Match all email addresses in this line
                    var matches = ExtractEmailsRegex.Matches(trimmedLine);
                    if (matches.Count == 0)
                    {
                        result.InvalidCount++;
                        continue;
                    }

                    foreach (Match match in matches)
                    {
                        result.TotalProcessed++;
                        var email = match.Value.Trim().ToLowerInvariant();

                        // Try extracting name if format is "John Doe <john@gmail.com>" or "John Doe, john@gmail.com"
                        string? extractedName = null;
                        if (trimmedLine.Contains("<") && trimmedLine.Contains(">"))
                        {
                            var namePart = trimmedLine.Substring(0, trimmedLine.IndexOf('<')).Trim(' ', '"', '\'', ',');
                            if (!string.IsNullOrWhiteSpace(namePart) && !namePart.Contains("@"))
                            {
                                extractedName = namePart;
                            }
                        }
                        else if (trimmedLine.Contains(",") || trimmedLine.Contains(";"))
                        {
                            var tokens = trimmedLine.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var token in tokens)
                            {
                                var cleanToken = token.Trim();
                                if (!cleanToken.Contains("@") && cleanToken.Length > 2 && !Regex.IsMatch(cleanToken, @"^[0-9\+\-\s]+$"))
                                {
                                    extractedName = cleanToken;
                                    break;
                                }
                            }
                        }

                        if (existingContacts.TryGetValue(email, out var existing))
                        {
                            if (!string.IsNullOrWhiteSpace(extractedName) && string.IsNullOrWhiteSpace(existing.FullName))
                                existing.FullName = extractedName;
                            if (!string.IsNullOrWhiteSpace(defaultSchool) && string.IsNullOrWhiteSpace(existing.SchoolName))
                                existing.SchoolName = defaultSchool;
                            if (!string.IsNullOrWhiteSpace(defaultRole) && string.IsNullOrWhiteSpace(existing.RoleTitle))
                                existing.RoleTitle = defaultRole;
                            if (defaultTag != "General" && existing.Tag == "General")
                                existing.Tag = defaultTag;

                            result.UpdatedCount++;
                        }
                        else
                        {
                            var newContact = new MarketingContact
                            {
                                Email = email,
                                FullName = extractedName,
                                RoleTitle = defaultRole,
                                SchoolName = defaultSchool,
                                Tag = string.IsNullOrWhiteSpace(defaultTag) ? "General" : defaultTag,
                                Source = "WordPaste",
                                IsSubscribed = true,
                                CreatedAtUtc = DateTime.UtcNow
                            };
                            _context.MarketingContacts.Add(newContact);
                            existingContacts[email] = newContact;
                            result.ImportedCount++;
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add($"Error parsing pasted text: {ex.Message}");
            }

            return result;
        }

        public async Task<MarketingDispatchResult> ExecuteCampaignAsync(
            long campaignId, 
            string? singleTestEmail = null, 
            string? operatorUser = null)
        {
            var result = new MarketingDispatchResult();
            var campaign = await _context.MarketingCampaigns
                .Include(c => c.Template)
                .FirstOrDefaultAsync(c => c.Id == campaignId);

            if (campaign == null)
            {
                result.Message = $"Campaign with ID {campaignId} was not found.";
                return result;
            }

            var rawHtml = !string.IsNullOrWhiteSpace(campaign.BodyHtml)
                ? campaign.BodyHtml
                : campaign.Template?.BodyHtml ?? "<p>No content provided</p>";

            // If single test email is specified, dispatch to only that email
            if (!string.IsNullOrWhiteSpace(singleTestEmail))
            {
                var mockContact = new MarketingContact
                {
                    Email = singleTestEmail,
                    FullName = "Test Recipient",
                    SchoolName = "Demo Model College",
                    RoleTitle = "School Proprietor",
                    State = "Lagos"
                };

                var renderedHtml = RenderEmailTemplate(rawHtml, mockContact, campaign.Subject);
                await _emailLoggingService.LogAndDumpEmailAsync(
                    recipientEmail: singleTestEmail,
                    subject: $"[TEST] {campaign.Subject}",
                    bodyHtml: renderedHtml,
                    category: "MarketingTest",
                    senderEmail: campaign.SenderEmail);

                result.Success = true;
                result.TotalRecipients = 1;
                result.DeliveredCount = 1;
                result.Message = $"Test email sent successfully to {singleTestEmail}. Check Email Outbox Log to preview.";
                return result;
            }

            // Normal campaign execution: fetch active recipients
            IQueryable<MarketingContact> query = _context.MarketingContacts.Where(c => c.IsSubscribed);

            if (!string.IsNullOrWhiteSpace(campaign.TargetTag) && campaign.TargetTag != "All")
            {
                query = query.Where(c => c.Tag == campaign.TargetTag);
            }

            var recipients = await query.ToListAsync();
            if (recipients.Count == 0)
            {
                result.Message = $"No subscribed contacts found for audience tag '{campaign.TargetTag}'.";
                return result;
            }

            campaign.Status = "Executing";
            campaign.ExecutionStartedUtc = DateTime.UtcNow;
            campaign.TotalRecipients = recipients.Count;
            await _context.SaveChangesAsync();

            int delivered = 0;
            int failed = 0;

            foreach (var contact in recipients)
            {
                try
                {
                    var renderedHtml = RenderEmailTemplate(rawHtml, contact, campaign.Subject);
                    
                    // Dump to Email Outbox Log for full platform visibility
                    await _emailLoggingService.LogAndDumpEmailAsync(
                        recipientEmail: contact.Email,
                        subject: campaign.Subject,
                        bodyHtml: renderedHtml,
                        recipientName: contact.FullName,
                        category: "MarketingBroadcast",
                        senderEmail: campaign.SenderEmail);

                    contact.LastEmailedUtc = DateTime.UtcNow;
                    delivered++;
                }
                catch (Exception)
                {
                    failed++;
                }
            }

            campaign.DeliveredCount = delivered;
            campaign.FailedCount = failed;
            campaign.Status = "Completed";
            campaign.ExecutionCompletedUtc = DateTime.UtcNow;
            campaign.ExecutionSummary = $"Broadcast completed. Delivered: {delivered}, Failed: {failed}.";

            await _context.SaveChangesAsync();

            // Log Audit Event
            await _auditLogService.LogAsync(
                action: "MarketingCampaignExecuted",
                details: $"Executed broadcast '{campaign.Title}' to {delivered} contacts (Tag: {campaign.TargetTag}).",
                userName: operatorUser ?? campaign.CreatedBy ?? "Admin",
                area: "Marketing");

            result.Success = true;
            result.TotalRecipients = recipients.Count;
            result.DeliveredCount = delivered;
            result.FailedCount = failed;
            result.Message = $"Successfully sent campaign '{campaign.Title}' to {delivered} contacts!";

            return result;
        }

        public string RenderEmailTemplate(string templateHtml, MarketingContact contact, string? overrideSubject = null)
        {
            if (string.IsNullOrWhiteSpace(templateHtml)) return string.Empty;

            var fullName = !string.IsNullOrWhiteSpace(contact.FullName) ? contact.FullName : "School Administrator";
            var schoolName = !string.IsNullOrWhiteSpace(contact.SchoolName) ? contact.SchoolName : "Your Esteemed School";
            var role = !string.IsNullOrWhiteSpace(contact.RoleTitle) ? contact.RoleTitle : "Educator";
            var state = !string.IsNullOrWhiteSpace(contact.State) ? contact.State : "Nigeria";
            var currentYear = DateTime.UtcNow.Year.ToString();

            var rendered = templateHtml
                .Replace("{{FullName}}", fullName)
                .Replace("{{FirstName}}", fullName.Split(' ')[0])
                .Replace("{{SchoolName}}", schoolName)
                .Replace("{{Email}}", contact.Email)
                .Replace("{{Role}}", role)
                .Replace("{{State}}", state)
                .Replace("{{CurrentYear}}", currentYear);

            return rendered;
        }

        public async Task<byte[]> ExportContactsExcelAsync(string? tagFilter = null)
        {
            IQueryable<MarketingContact> query = _context.MarketingContacts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(tagFilter) && tagFilter != "All")
            {
                query = query.Where(c => c.Tag == tagFilter);
            }

            var contacts = await query.OrderByDescending(c => c.CreatedAtUtc).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Marketing Contacts");

            // Title Header
            worksheet.Cell(1, 1).Value = "Exwhyzee SP - Marketing Audience Contacts";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;

            // Table Headers
            worksheet.Cell(3, 1).Value = "Email Address";
            worksheet.Cell(3, 2).Value = "Full Name";
            worksheet.Cell(3, 3).Value = "Phone Number";
            worksheet.Cell(3, 4).Value = "School / Institution";
            worksheet.Cell(3, 5).Value = "Role / Title";
            worksheet.Cell(3, 6).Value = "State / Location";
            worksheet.Cell(3, 7).Value = "Tag / Segment";
            worksheet.Cell(3, 8).Value = "Subscribed";
            worksheet.Cell(3, 9).Value = "Source";
            worksheet.Cell(3, 10).Value = "Created Date";
            worksheet.Cell(3, 11).Value = "Last Emailed";

            var headerRange = worksheet.Range(3, 1, 3, 11);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0b2239");
            headerRange.Style.Font.FontColor = XLColor.White;

            int row = 4;
            foreach (var c in contacts)
            {
                worksheet.Cell(row, 1).Value = c.Email;
                worksheet.Cell(row, 2).Value = c.FullName ?? "";
                worksheet.Cell(row, 3).Value = c.PhoneNumber ?? "";
                worksheet.Cell(row, 4).Value = c.SchoolName ?? "";
                worksheet.Cell(row, 5).Value = c.RoleTitle ?? "";
                worksheet.Cell(row, 6).Value = c.State ?? "";
                worksheet.Cell(row, 7).Value = c.Tag;
                worksheet.Cell(row, 8).Value = c.IsSubscribed ? "Yes" : "No";
                worksheet.Cell(row, 9).Value = c.Source;
                worksheet.Cell(row, 10).Value = c.CreatedAtUtc.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cell(row, 11).Value = c.LastEmailedUtc?.ToString("yyyy-MM-dd HH:mm") ?? "Never";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
