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
using Microsoft.EntityFrameworkCore;
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

        public async Task<BackupResult> GenerateDatabaseBackupAsync(TenantSchool tenant, string? initiatedBy = null, bool recordHistory = true)
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

                    if (recordHistory)
                    {
                        await RecordHistoryAsync(new TenantBackupHistory
                        {
                            TenantSchoolId = tenant.Id,
                            SchoolName = tenant.SchoolName,
                            Subdomain = tenant.SubDomain,
                            BackupDate = DateTime.UtcNow,
                            FileName = fileName,
                            FileSize = formattedSize,
                            BackupType = "Local Download",
                            InitiatedBy = initiatedBy ?? "Master Admin",
                            Status = "Success",
                            Notes = "Generated local database snapshot stream"
                        });
                    }

                    return new BackupResult
                    {
                        Success = true,
                        Message = $"Native database backup generated successfully for {tenant.SchoolName}.",
                        FileBytes = fileBytes,
                        FileName = fileName,
                        FileSizeFormatted = formattedSize
                    };
                }

                if (recordHistory)
                {
                    await RecordHistoryAsync(new TenantBackupHistory
                    {
                        TenantSchoolId = tenant.Id,
                        SchoolName = tenant.SchoolName,
                        Subdomain = tenant.SubDomain,
                        BackupDate = DateTime.UtcNow,
                        FileName = fileName,
                        BackupType = "Local Download",
                        InitiatedBy = initiatedBy ?? "Master Admin",
                        Status = "Failed",
                        Notes = "SQL backup completed but output file was not found on disk."
                    });
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

                if (recordHistory)
                {
                    try
                    {
                        await RecordHistoryAsync(new TenantBackupHistory
                        {
                            TenantSchoolId = tenant.Id,
                            SchoolName = tenant.SchoolName,
                            Subdomain = tenant.SubDomain,
                            BackupDate = DateTime.UtcNow,
                            BackupType = "Local Download",
                            InitiatedBy = initiatedBy ?? "Master Admin",
                            Status = "Failed",
                            Notes = $"Error: {ex.Message}"
                        });
                    }
                    catch { /* best effort */ }
                }

                return new BackupResult
                {
                    Success = false,
                    Message = $"Backup generation failed: {ex.Message}"
                };
            }
        }

        public async Task<BackupResult> UploadToCloudinaryAsync(TenantSchool tenant, byte[] fileBytes, string fileName, string? initiatedBy = null)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return new BackupResult { Success = false, Message = "Backup payload is empty. Cannot upload to Cloudinary." };
            }

            // 1. Resolve Cloudinary Credentials from Environment or Configuration
            var (cloudName, apiKey, apiSecret) = GetCloudinaryCredentials();

            if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            {
                await RecordHistoryAsync(new TenantBackupHistory
                {
                    TenantSchoolId = tenant.Id,
                    SchoolName = tenant.SchoolName,
                    Subdomain = tenant.SubDomain,
                    BackupDate = DateTime.UtcNow,
                    FileName = fileName,
                    BackupType = "Cloudinary Vault",
                    InitiatedBy = initiatedBy ?? "Master Admin",
                    Status = "Failed",
                    Notes = "Cloudinary credentials missing in .env or configuration"
                });

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

                    await RecordHistoryAsync(new TenantBackupHistory
                    {
                        TenantSchoolId = tenant.Id,
                        SchoolName = tenant.SchoolName,
                        Subdomain = tenant.SubDomain,
                        BackupDate = DateTime.UtcNow,
                        FileName = fileName,
                        BackupType = "Cloudinary Vault",
                        InitiatedBy = initiatedBy ?? "Master Admin",
                        Status = "Failed",
                        Notes = $"Cloudinary API status {response.StatusCode}"
                    });

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

                // 4. Record to backup history log
                await RecordHistoryAsync(new TenantBackupHistory
                {
                    TenantSchoolId = tenant.Id,
                    SchoolName = tenant.SchoolName,
                    Subdomain = tenant.SubDomain,
                    BackupDate = DateTime.UtcNow,
                    FileUrl = secureUrl,
                    FileName = fileName,
                    FileSize = formattedSize,
                    BackupType = "Cloudinary Vault",
                    InitiatedBy = initiatedBy ?? "Master Admin",
                    Status = "Success",
                    Notes = "Securely synced to Cloudinary vault"
                });

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

                try
                {
                    await RecordHistoryAsync(new TenantBackupHistory
                    {
                        TenantSchoolId = tenant.Id,
                        SchoolName = tenant.SchoolName,
                        Subdomain = tenant.SubDomain,
                        BackupDate = DateTime.UtcNow,
                        FileName = fileName,
                        BackupType = "Cloudinary Vault",
                        InitiatedBy = initiatedBy ?? "Master Admin",
                        Status = "Failed",
                        Notes = $"Exception: {ex.Message}"
                    });
                }
                catch { /* best effort */ }

                return new BackupResult
                {
                    Success = false,
                    Message = $"Cloudinary upload failed: {ex.Message}"
                };
            }
        }

        public async Task<List<TenantBackupHistory>> GetBackupHistoryAsync(int? tenantSchoolId = null)
        {
            var query = _context.TenantBackupHistories.AsQueryable();

            if (tenantSchoolId.HasValue && tenantSchoolId.Value > 0)
            {
                query = query.Where(h => h.TenantSchoolId == tenantSchoolId.Value);
            }

            return await query.OrderByDescending(h => h.BackupDate).ToListAsync();
        }

        public async Task<TenantBackupHistory> RecordHistoryAsync(TenantBackupHistory history)
        {
            try
            {
                _context.TenantBackupHistories.Add(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist tenant backup history entry.");
            }
            return history;
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
