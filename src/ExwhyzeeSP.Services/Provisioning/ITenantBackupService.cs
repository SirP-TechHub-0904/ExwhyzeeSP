using System;
using System.IO;
using System.Threading.Tasks;
using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Services.Provisioning
{
    public class BackupResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public byte[]? FileBytes { get; set; }
        public string? FileName { get; set; }
        public string? FileSizeFormatted { get; set; }
        public string? CloudUrl { get; set; }
    }

    public interface ITenantBackupService
    {
        Task<BackupResult> GenerateDatabaseBackupAsync(TenantSchool tenant);
        Task<BackupResult> UploadToCloudinaryAsync(TenantSchool tenant, byte[] fileBytes, string fileName);
    }
}
