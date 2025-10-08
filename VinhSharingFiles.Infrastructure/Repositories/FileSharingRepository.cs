using Microsoft.EntityFrameworkCore;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Infrastructure.Data;

namespace VinhSharingFiles.Infrastructure.Repositories;

public class FileSharingRepository(VinhSharingDbContext context) : IFileSharingRepository
{
    private readonly VinhSharingDbContext _context = context;

    public async Task<int> AddFileAsync(FileSharing fileInfo)
    {
        _context.FileSharings.Add(fileInfo);
        await _context.SaveChangesAsync();
        return fileInfo.Id;        
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

    public async Task<IEnumerable<FileSharing>> GetAllFiles(int userId)
    {
        return await _context.FileSharings
            .Where(x => x.UserId == userId)            
            .ToListAsync();
    }

    public async Task<FileSharing?> GetFileByIdAsync(int id)
        => await _context.FileSharings.FindAsync(id);

    public async Task<bool> GetFileNameExistingAsync(string fileName)
    {
        return await _context.FileSharings
            .Where(x => x.FileName == fileName)
            .AnyAsync();
    }

    public async Task UpdateFileAsync(FileSharing fileInfo)
    {
        _context.FileSharings.Update(fileInfo);
        await _context.SaveChangesAsync();
    }
}
