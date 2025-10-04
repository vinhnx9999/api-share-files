using VinhSharingFiles.Infrastructure.Data;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VinhSharingFiles.Domain.DTOs;

namespace VinhSharingFiles.Infrastructure.Repositories;

public class FileSharingRepository(VinhSharingDbContext context) : IFileSharingRepository
{
    private readonly VinhSharingDbContext _context = context;

    public async Task<int> AddFileAsync(FileSharing fileInfo)
    {
        try
        {
            if (fileInfo.FileName != "STORE_TEXT_IN_DB")
            {
                var existingFile = await _context.FileSharings.FirstOrDefaultAsync(f => f.FileName == fileInfo.FileName && f.UserId == fileInfo.UserId);
                if (existingFile != null)
                {
                    throw new ArgumentException("A file with the same name already exists for this user.");
                }
            }               

            _context.FileSharings.Add(fileInfo);
            await _context.SaveChangesAsync();

            return fileInfo.Id;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error checking for existing file: " + ex.Message);
        }
    }

    public async Task DeleteFileByIdAsync(int id)
    {
        var fileInfo = await _context.FileSharings.FindAsync(id);
        if (fileInfo != null)
        {
            _context.FileSharings.Remove(fileInfo);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<FileDto>> GetAllFiles(int userId)
    {
        return await _context.FileSharings
            .Where(x => x.UserId == userId)
            .Select( x => new FileDto
            {
                Id = x.Id,
                FileSize = x.FileSize, 
                FileName = x.FileName,
                FilePath = x.FilePath,
                ContentType = x.FileType,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
            })
            .ToListAsync();
    }

    public async Task<FileSharing> GetFileByIdAsync(int id)
    {
        return await _context.FileSharings.FindAsync(id) ?? throw new Exception("File not found");
    }

    public async Task UpdateFileAsync(FileSharing fileInfo)
    {
        _context.FileSharings.Update(fileInfo);
        await _context.SaveChangesAsync();
    }
}
