using ExwhyzeeSP.Core.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Services
{
    public class StorageService : IStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public StorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<FileReturnResponseDto> MainUploadFileReturnUrlAsync(string old_key, IFormFile file)
        {
            var response = new FileReturnResponseDto();
            if (file == null || file.Length == 0)
            {
                return response;
            }

            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                response.Url = $"/uploads/{uniqueFileName}";
                response.Key = uniqueFileName;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<FileReturnResponseDto> UploadFileReturnUrlAsync(S3Object obj, AwsCredentials awsCredentialsValues, string old_key)
        {
            var response = new FileReturnResponseDto();
            if (obj?.InputStream == null)
            {
                return response;
            }

            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{obj.Name ?? "file"}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    if (obj.InputStream.CanSeek)
                    {
                        obj.InputStream.Position = 0;
                    }
                    await obj.InputStream.CopyToAsync(fileStream);
                }

                response.Url = $"/uploads/{uniqueFileName}";
                response.Key = uniqueFileName;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        public Task<FileReturnResponseDto> DeleteObjectAsync(AwsCredentials awsCredentialsValues, string bucket, string key)
        {
            var response = new FileReturnResponseDto();
            if (!string.IsNullOrEmpty(key))
            {
                var filePath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", key);
                if (File.Exists(filePath))
                {
                    try { File.Delete(filePath); } catch { }
                }
            }
            response.Message = "Success";
            return Task.FromResult(response);
        }

        public Task<string> BucketName()
        {
            return Task.FromResult("local-uploads");
        }
    }
}
