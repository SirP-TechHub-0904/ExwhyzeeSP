using ExwhyzeeSP.Core.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Services
{
    public interface IStorageService
    {
        Task<FileReturnResponseDto> UploadFileReturnUrlAsync(S3Object obj, AwsCredentials awsCredentialsValues, string old_key);
        Task<FileReturnResponseDto> MainUploadFileReturnUrlAsync(string old_key, IFormFile file);
        Task<FileReturnResponseDto> DeleteObjectAsync(AwsCredentials awsCredentialsValues, string bucket, string key);
        Task<string> BucketName();
    }
}
