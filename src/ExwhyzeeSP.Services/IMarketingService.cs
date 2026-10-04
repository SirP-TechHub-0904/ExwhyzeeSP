using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;
using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Services
{
    public class MarketingImportResult
    {
        public int TotalProcessed { get; set; }
        public int ImportedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedDuplicateCount { get; set; }
        public int InvalidCount { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
    }

    public class MarketingDispatchResult
    {
        public bool Success { get; set; }
        public int TotalRecipients { get; set; }
        public int DeliveredCount { get; set; }
        public int FailedCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public interface IMarketingService
    {
        Task<MarketingImportResult> ImportContactsFromExcelAsync(Stream fileStream, string defaultTag = "General");
        Task<MarketingImportResult> ImportContactsFromRawTextAsync(string rawText, string defaultTag = "General", string? defaultRole = null, string? defaultSchool = null);
        Task<MarketingDispatchResult> ExecuteCampaignAsync(long campaignId, string? singleTestEmail = null, string? operatorUser = null);
        Task<byte[]> ExportContactsExcelAsync(string? tagFilter = null);
        string RenderEmailTemplate(string templateHtml, MarketingContact contact, string? overrideSubject = null);
    }
}
