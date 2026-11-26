using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Data.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Services
{
    public class StorageService : IStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public StorageService(
            IWebHostEnvironment environment,
            IConfiguration config,
            ApplicationDbContext context)
        {
            _environment = environment;
            _config = config;
            _context = context;
        }

        private (string AccessKey, string SecretKey) ResolveCredentials(AwsCredentials? creds = null)
        {
            var accessKey = !string.IsNullOrWhiteSpace(creds?.AccessKey)
                ? creds.AccessKey
                : (_config["AwsConfiguration:AWSAccessKey"] ?? Environment.GetEnvironmentVariable("AwsConfiguration__AWSAccessKey") ?? "");
            var secretKey = !string.IsNullOrWhiteSpace(creds?.SecretKey)
                ? creds.SecretKey
                : (_config["AwsConfiguration:AWSSecretKey"] ?? Environment.GetEnvironmentVariable("AwsConfiguration__AWSSecretKey") ?? "");
            return (accessKey, secretKey);
        }

        private static AmazonS3Client CreateS3Client(string accessKey, string secretKey)
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast1
            };
            return new AmazonS3Client(credentials, s3Config);
        }

        public async Task<string> BucketName()
        {
            var configured = _config["AwsConfiguration:BucketName"]
                ?? Environment.GetEnvironmentVariable("AwsConfiguration__BucketName");

            if (!string.IsNullOrWhiteSpace(configured))
                return configured;

            try
            {
                var superSetting = await _context.SuperSettings.AsNoTracking().FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(superSetting?.BucketName))
                    return superSetting.BucketName;
            }
            catch
            {
            }

            return "juray2023";
        }

        public async Task<FileReturnResponseDto> GetUrlAsync(AwsCredentials awsCredentialsValues, string key)
        {
            var response = new FileReturnResponseDto();
            if (string.IsNullOrWhiteSpace(key))
            {
                response.Message = "Empty key";
                return response;
            }

            var (accessKey, secretKey) = ResolveCredentials(awsCredentialsValues);
            if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
            {
                response.Key = key;
                response.Message = "Storage credentials not configured";
                return response;
            }

            try
            {
                var bucket = await BucketName();
                using var client = CreateS3Client(accessKey, secretKey);
                var req = new GetPreSignedUrlRequest
                {
                    BucketName = bucket,
                    Key = key,
                    Expires = DateTime.UtcNow.AddYears(3)
                };

                response.Key = key;
                response.Url = client.GetPreSignedURL(req);
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<FileReturnResponseDto> MainUploadFileReturnUrlAsync(string old_key, IFormFile file)
        {
            var response = new FileReturnResponseDto();
            if (file == null || file.Length == 0)
            {
                response.Message = "No file provided";
                return response;
            }

            var (accessKey, secretKey) = ResolveCredentials();
            if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
            {
                response.Message = "Storage credentials missing. Upload aborted without fallback.";
                return response;
            }

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var ext = Path.GetExtension(file.FileName);
            var obj = new ExwhyzeeSP.Core.Dtos.S3Object
            {
                BucketName = await BucketName(),
                InputStream = ms,
                Name = $"{Guid.NewGuid()}{ext}"
            };

            var creds = new AwsCredentials
            {
                AccessKey = accessKey,
                SecretKey = secretKey
            };

            return await UploadFileReturnUrlAsync(obj, creds, old_key);
        }

        public async Task<FileReturnResponseDto> UploadFileReturnUrlAsync(ExwhyzeeSP.Core.Dtos.S3Object obj, AwsCredentials awsCredentialsValues, string old_key)
        {
            var response = new FileReturnResponseDto();
            if (obj?.InputStream == null)
            {
                response.Message = "Empty file stream";
                return response;
            }

            var (accessKey, secretKey) = ResolveCredentials(awsCredentialsValues);
            if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
            {
                try
                {
                    var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var uploadsDir = Path.Combine(webRoot, "uploads");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }
                    var localPath = Path.Combine(uploadsDir, obj.Name);
                    if (obj.InputStream.CanSeek) obj.InputStream.Position = 0;
                    using (var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write))
                    {
                        await obj.InputStream.CopyToAsync(fs);
                    }
                    response.Url = $"/uploads/{obj.Name}";
                    response.Key = obj.Name;
                    response.Message = "200";
                    return response;
                }
                catch (Exception localEx)
                {
                    response.Message = $"Storage credentials missing and local storage failed: {localEx.Message}";
                    return response;
                }
            }

            try
            {
                var bucket = !string.IsNullOrWhiteSpace(obj.BucketName) ? obj.BucketName : await BucketName();
                using var client = CreateS3Client(accessKey, secretKey);

                if (!string.IsNullOrWhiteSpace(old_key))
                {
                    try
                    {
                        await client.DeleteObjectAsync(new DeleteObjectRequest { BucketName = bucket, Key = old_key });
                    }
                    catch
                    {
                    }
                }

                if (obj.InputStream.CanSeek)
                {
                    obj.InputStream.Position = 0;
                }

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = obj.InputStream,
                    Key = obj.Name,
                    BucketName = bucket,
                    CannedACL = S3CannedACL.NoACL
                };

                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(uploadRequest);

                var urlReq = new GetPreSignedUrlRequest
                {
                    BucketName = bucket,
                    Key = obj.Name,
                    Expires = DateTime.UtcNow.AddYears(3)
                };

                response.Url = client.GetPreSignedURL(urlReq);
                response.Key = obj.Name;
                response.Message = "200";
                return response;
            }
            catch (Exception ex)
            {
                try
                {
                    var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var uploadsDir = Path.Combine(webRoot, "uploads");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }
                    var localPath = Path.Combine(uploadsDir, obj.Name);
                    if (obj.InputStream.CanSeek) obj.InputStream.Position = 0;
                    using (var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write))
                    {
                        await obj.InputStream.CopyToAsync(fs);
                    }
                    response.Url = $"/uploads/{obj.Name}";
                    response.Key = obj.Name;
                    response.Message = "200";
                    return response;
                }
                catch (Exception localEx)
                {
                    response.Message = $"Cloud upload failed: {ex.Message}; Local fallback failed: {localEx.Message}";
                    return response;
                }
            }
        }

        public async Task<FileReturnResponseDto> DeleteObjectAsync(AwsCredentials awsCredentialsValues, string bucket, string key)
        {
            var response = new FileReturnResponseDto();
            if (string.IsNullOrWhiteSpace(key))
            {
                response.Message = "Success";
                return response;
            }

            var (accessKey, secretKey) = ResolveCredentials(awsCredentialsValues);
            if (!string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(secretKey))
            {
                try
                {
                    var targetBucket = !string.IsNullOrWhiteSpace(bucket) ? bucket : await BucketName();
                    using var client = CreateS3Client(accessKey, secretKey);
                    await client.DeleteObjectAsync(new DeleteObjectRequest
                    {
                        BucketName = targetBucket,
                        Key = key
                    });
                    response.Message = "Success";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    return response;
                }
            }

            var filePath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", key);
            if (File.Exists(filePath))
            {
                try { File.Delete(filePath); } catch { }
            }

            response.Message = "Success";
            return response;
        }
    }
}
