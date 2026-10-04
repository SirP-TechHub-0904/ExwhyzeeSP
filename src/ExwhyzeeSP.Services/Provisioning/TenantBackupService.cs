using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Services.Provisioning
{
    public class TenantBackupService : ITenantBackupService
    {
        private readonly MasterDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TenantBackupService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public TenantBackupService(
            MasterDbContext context,
            IConfiguration configuration,
            ILogger<TenantBackupService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<BackupResult> GenerateDatabaseBackupAsync(TenantSchool tenant)
        {
            if (tenant == null || string.IsNullOrWhiteSpace(tenant.ConnectionString))
            {
                return new BackupResult { Success = false, Message = "Invalid tenant school or missing connection string." };
            }

            try
            {
                var builder = new SqlConnectionStringBuilder(tenant.ConnectionString);
                var databaseName = builder.InitialCatalog;
                if (string.IsNullOrWhiteSpace(databaseName))
                {
                    databaseName = $"ExwhyzeeSP_{tenant.SubDomain ?? tenant.Id.ToString()}";
                }

                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var cleanSub = (tenant.SubDomain ?? $"tenant_{tenant.Id}").Replace("-", "_");
                var fileName = $"{cleanSub}_backup_{timestamp}.bak";
                var tempPath = Path.Combine(Path.GetTempPath(), fileName);

                // 1. Attempt native SQL Server BACKUP DATABASE
                using (var connection = new SqlConnection(tenant.ConnectionString))
                {
                    await connection.OpenAsync();

                    var backupSql = $@"
                        BACKUP DATABASE [{databaseName}] 
                        TO DISK = @BackupPath 
                        WITH FORMAT, INIT, COPY_ONLY, SKIP, NOREWIND, NOUNLOAD, STATS = 10;
                    ";

                    using (var cmd = new SqlCommand(backupSql, connection))
                    {
                        cmd.CommandTimeout = 300; // 5 minutes for large DBs
                        cmd.Parameters.AddWithValue("@BackupPath", tempPath);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                if (File.Exists(tempPath))
                {
                    var fileBytes = await File.ReadAllBytesAsync(tempPath);
                    var fileLength = new FileInfo(tempPath).Length;
                    var formattedSize = FormatBytes(fileLength);

                    // Clean up local temp file
                    try { File.Delete(tempPath); } catch { /* best effort */ }

                    return new BackupResult
                    {
                        Success = true,
                        Message = $"Native database backup generated successfully for {tenant.SchoolName}.",
                        FileBytes = fileBytes,
                        FileName = fileName,
                        FileSizeFormatted = formattedSize
                    };
                }

                return new BackupResult
                {
                    Success = false,
                    Message = "SQL backup completed but output file was not found on disk."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate SQL backup for tenant {SchoolName}", tenant.SchoolName);
                return new BackupResult
                {
                    Success = false,
                    Message = $"Backup generation failed: {ex.Message}"
                };
            }
        }

        public async Task<BackupResult> UploadToCloudinaryAsync(TenantSchool tenant, byte[] fileBytes, string fileName)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return new BackupResult { Success = false, Message = "Backup payload is empty. Cannot upload to Cloudinary." };
            }

            // 1. Resolve Cloudinary Credentials from Environment or Configuration
            var (cloudName, apiKey, apiSecret) = GetCloudinaryCredentials();

            if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            {
                return new BackupResult
                {
                    Success = false,
                    Message = "Cloudinary is not configured. Please add CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY, and CLOUDINARY_API_SECRET (or CLOUDINARY_URL) to your .env configuration."
                };
            }

            try
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var folder = "exwhyzee_db_backups";
                var cleanSub = (tenant.SubDomain ?? $"tenant_{tenant.Id}").Replace("-", "_");
                var publicId = $"{folder}/{cleanSub}_{DateTime.UtcNow:yyyyMMdd_HHmmss}";

                // 2. Compute SHA-1 Signature for Cloudinary REST API
                // String to sign: folder={folder}&public_id={publicId}&timestamp={timestamp}{apiSecret}
                var signParams = new SortedDictionary<string, string>
                {
                    { "folder", folder },
                    { "public_id", publicId },
                    { "timestamp", timestamp }
                };

                var stringToSign = string.Join("&", signParams.Select(kv => $"{kv.Key}={kv.Value}")) + apiSecret;
                var signature = HashSha1(stringToSign);

                var uploadUrl = $"https://api.cloudinary.com/v1_1/{cloudName}/raw/upload";

                using var client = _httpClientFactory.CreateClient();
                using var form = new MultipartFormDataContent();

                form.Add(new ByteArrayContent(fileBytes), "file", fileName);
                form.Add(new StringContent(apiKey), "api_key");
                form.Add(new StringContent(timestamp), "timestamp");
                form.Add(new StringContent(folder), "folder");
                form.Add(new StringContent(publicId), "public_id");
                form.Add(new StringContent(signature), "signature");

                var response = await client.PostAsync(uploadUrl, form);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Cloudinary upload failed with status {StatusCode}: {Response}", response.StatusCode, responseContent);
                    return new BackupResult
                    {
                        Success = false,
                        Message = $"Cloudinary API error ({response.StatusCode}): {responseContent}"
                    };
                }

                using var doc = JsonDocument.Parse(responseContent);
                var secureUrl = doc.RootElement.TryGetProperty("secure_url", out var urlProp)
                    ? urlProp.GetString()
                    : (doc.RootElement.TryGetProperty("url", out var plainUrlProp) ? plainUrlProp.GetString() : null);

                if (string.IsNullOrWhiteSpace(secureUrl))
                {
                    return new BackupResult
                    {
                        Success = false,
                        Message = "Upload succeeded but no secure URL was returned by Cloudinary."
                    };
                }

                var formattedSize = FormatBytes(fileBytes.Length);

                // 3. Update Tenant School metadata
                tenant.LastBackupDate = DateTime.UtcNow;
                tenant.LastBackupUrl = secureUrl;
                tenant.LastBackupSize = formattedSize;
                await _context.SaveChangesAsync();

                return new BackupResult
                {
                    Success = true,
                    Message = $"Backup archive securely uploaded and vaulted to Cloudinary for {tenant.SchoolName}.",
                    CloudUrl = secureUrl,
                    FileSizeFormatted = formattedSize,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while uploading tenant backup to Cloudinary.");
                return new BackupResult
                {
                    Success = false,
                    Message = $"Cloudinary upload failed: {ex.Message}"
                };
            }
        }

        private (string? CloudName, string? ApiKey, string? ApiSecret) GetCloudinaryCredentials()
        {
            var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME")
                ?? _configuration["Cloudinary:CloudName"];
            var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")
                ?? _configuration["Cloudinary:ApiKey"];
            var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
                ?? _configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            {
                var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL")
                    ?? _configuration["Cloudinary:Url"];

                if (!string.IsNullOrWhiteSpace(cloudinaryUrl) && cloudinaryUrl.StartsWith("cloudinary://"))
                {
                    try
                    {
                        var uri = new Uri(cloudinaryUrl);
                        cloudName = uri.Host;
                        var userInfo = uri.UserInfo.Split(':');
                        if (userInfo.Length == 2)
                        {
                            apiKey = userInfo[0];
                            apiSecret = userInfo[1];
                        }
                    }
                    catch { /* fallback */ }
                }
            }

            return (cloudName, apiKey, apiSecret);
        }

        private static string HashSha1(string input)
        {
            using var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n1} {suffixes[counter]}";
        }
    }
}
