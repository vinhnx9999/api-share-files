using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Application.Interfaces
{
    public interface IFileSharingRepository 
    {
        Task<FileSharing> GetFileByIdAsync(int id);
        Task<int> AddFileAsync(FileSharing fileInfo);
        Task UpdateFileAsync(FileSharing fileInfo);
        Task DeleteFileByIdAsync(int id);
        Task<IEnumerable<FileDto>> GetAllFiles(int userId);
    }
}
