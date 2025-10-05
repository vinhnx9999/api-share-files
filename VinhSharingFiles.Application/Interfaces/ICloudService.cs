using Microsoft.AspNetCore.Http;
using VinhSharingFiles.Domain.DTOs;

namespace VinhSharingFiles.Application.Interfaces;

public interface ICloudService
{        
    Task<FileObjectDto> DownloadFileAsync(int fileId);
    Task<IEnumerable<FileDto>> GetAllFilesByUserIdAsync(int userId);
    Task<FileObjectDto> PreviewFileAsync(int fileId);
    Task<int> UploadFileAsync(int userId, IFormFile file, bool? deleteAfterDownload);
    Task<int> UploadTextFileAsync(int userId, string textData, bool? deleteAfterAccessed, bool isSensitive);
}
