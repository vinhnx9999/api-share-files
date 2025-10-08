using Microsoft.AspNetCore.Http;

namespace VinhSharingFiles.Application.Interfaces;

public interface IExternalService
{
    Task<string> MultipartUploadAsync(string fileName, IFormFile file, Dictionary<string, string> customMetadata);
    Task<string> UploadFileAsync(string fileName, IFormFile file, Dictionary<string, string> customMetadata);
    Task<Stream> DownloadFileAsync(string fileName);
    Task DeleteFileAsync(string fileName);
    Task<string> UploadTextFileAsync(string fileName, string contentToStore, Dictionary<string, string> customMetadata);
}