using Microsoft.AspNetCore.Http;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Application.DTOs;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.Application.Services;

public class FileSharingService(IExternalService externalService, IFileSharingRepository fileRepository) : IFileSharingService
{   
    private readonly IFileSharingRepository _fileRepository = fileRepository;
    private readonly IExternalService _externalService = externalService;

    public async Task<IEnumerable<FileDto>> GetAllFilesByUserIdAsync(int userId)
    {
        var data = await _fileRepository.GetAllFiles(userId);
        return data.Select(x => new FileDto
        {
            Id = x.Id,
            FileSize = x.FileSize,
            FileName = x.FileName,
            FilePath = x.FilePath,
            ContentType = x.FileType,
            Description = x.Description,
            CreatedAt = x.CreatedAt,
        });
    }

    public async Task<FileObjectDto> PreviewFileAsync(int fileId)
    {
        // Logic to get the file name from your database using the fileId
        var fileRecord = await _fileRepository.GetFileByIdAsync(fileId) ?? throw new Exception("File not found in the database.");

        if (fileRecord.FileName == FileVariables.STORE_TEXT_IN_DB)
        {
            return new FileObjectDto
            {                    
                ContentType = FileVariables.STORE_TEXT_IN_DB,
                FileId = fileId,
                Name = FileVariables.STORE_TEXT_IN_DB, //This is the display file name
                Description = fileRecord.Description // This is the text stored
            };
        }

        // Use display name if available, otherwise use the original file name
        string fileName = fileRecord.DisplayName ?? fileRecord.FileName;
        var responseStream = await _externalService.DownloadFileAsync(fileRecord.FileName);
        
        return new FileObjectDto
        {
            ContentType = fileRecord.FileType,
            FileId = fileId,
            Name = fileName, //This is the display file name
            Data = responseStream // This is the stream containing the file data
        };
    }

    public async Task<FileObjectDto> DownloadFileAsync(int fileId)
    {
        // Logic to get the file name from your database using the fileId
        var fileRecord = await _fileRepository.GetFileByIdAsync(fileId) ?? throw new Exception("File not found in the database.");
        if (fileRecord.FileName == FileVariables.STORE_TEXT_IN_DB)
        {
            return new FileObjectDto
            {
                ContentType = FileVariables.STORE_TEXT_IN_DB,
                FileId = fileId,
                Name = FileVariables.STORE_TEXT_IN_DB, //This is the display file name
                Description = fileRecord.Description // This is the text stored
            };
        }

        // Use display name if available, otherwise use the original file name
        string fileName = fileRecord.DisplayName ?? fileRecord.FileName;
        var responseStream = await _externalService.DownloadFileAsync(fileRecord.FileName);

        if (fileRecord.AutoDelete)
        {
            // If the file is marked as delete after download, remove it from the database after fetching
            await _fileRepository.DeleteFileByIdAsync(fileId);
            // Also delete from external storage via background job
            await _externalService.DeleteFileAsync(fileRecord.FileName);
        }

        return new FileObjectDto
        {
            ContentType = fileRecord.FileType,
            FileId = fileId,
            Name = fileName, //This is the display file name
            Data = responseStream // This is the stream containing the file data
        };
    }

    public async Task<int> UploadFileAsync(int userId, IFormFile file, bool? deleteAfterAccessed)
    {        
        string fileName = await EnsureFileNameValidAsync(file.FileName);
        var fileSize = file.Length;
        Dictionary<string, string> customMetadata = new()
        {
            { "UploadedBy", userId.ToString() },
            { "AutoDelete", (deleteAfterAccessed ?? false).ToString() },
            { "UploadDate", DateTime.UtcNow.ToString("o") } // ISO 8601 format
        };

        // If file size > 150 MB, use multipart upload
        bool isLargeFile = fileSize > FileVariables.MAX_FILE_SIZE_IN_MB * 1024 * 1024;
        fileName = isLargeFile ? 
            await _externalService.MultipartUploadAsync(fileName, file, customMetadata) : 
            await _externalService.UploadFileAsync(fileName, file, customMetadata);
          
        return await SaveFileToDatabase(userId, file.FileName, fileName, file.ContentType, file.Length, deleteAfterAccessed);            
    }

    private async Task<string> EnsureFileNameValidAsync(string fileName)
    {
        try
        {
            if (fileName == FileVariables.STORE_TEXT_IN_DB) return fileName;

            string str = ReplaceInvalidFileNameChars(fileName);
            bool fileExisting = await _fileRepository.GetFileNameExistingAsync(fileName);
            if (fileExisting)
            {
                return $"{Path.GetFileNameWithoutExtension(str)}_{DateTime.UtcNow.Ticks:X2}{Path.GetExtension(str)}";
            }
            return str;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error checking for existing file: " + ex.Message);
        }
    }

    //Create a text file and upload
    public async Task<int> UploadTextFileAsync(int userId, string contentToStore, bool? deleteAfterAccessed, bool isSensitive)
    {
        if (!isSensitive)
        {
            return await SaveTextToDatabase(userId, contentToStore, deleteAfterAccessed);
        }

        string fileName = $"data_{userId:X2}_{DateTime.UtcNow.Ticks:X2}.txt";
        // You can optionally set the content type, e.g., "text/plain".

        Dictionary<string, string> customMetadata = new()
        {
            { "UploadedBy", userId.ToString() },
            { "AutoDelete", (deleteAfterAccessed ?? false).ToString() },
            { "UploadDate", DateTime.UtcNow.ToString("o") } // ISO 8601 format
        };
        fileName = await _externalService.UploadTextFileAsync(fileName, contentToStore, customMetadata);
        return await SaveFileToDatabase(userId, fileName, fileName, "text/plain", 0, deleteAfterAccessed);
    }

    #region Private Methods

    private static string ReplaceInvalidFileNameChars(string fileName)
    {
        if(string.IsNullOrWhiteSpace(fileName))
            return "";

        char replacementChar = '_';
        return string.Join(replacementChar.ToString(), fileName.Split(Path.GetInvalidFileNameChars()));
    }

    private async Task<int> SaveTextToDatabase(int userId, string? contentToStore, bool? deleteAfterAccessed)
    {
        // Save file info to database
        FileSharing fileInfo = new()
        {
            Id = 0,
            UserId = userId,
            DisplayName = FileVariables.STORE_TEXT_IN_DB,
            FileName = FileVariables.STORE_TEXT_IN_DB,
            FileType = "",
            FilePath = "",
            FileSize = 0,
            Description = contentToStore,
            CreatedAt = DateTime.UtcNow,
            AutoDelete = deleteAfterAccessed ?? false
        };

        var fileId = await _fileRepository.AddFileAsync(fileInfo);

        // Return the file ID or any other relevant information
        return fileId;
    }

    private async Task<int> SaveFileToDatabase(int userId, string displayName, string fileName, 
        string contentType, long fileSize, bool? deleteAfterDownload)
    {
        // Save file info to database
        FileSharing fileInfo = new()
        {
            Id = 0,
            UserId = userId,
            DisplayName = displayName,
            FileType = contentType,
            FilePath = fileName,
            FileName = fileName,
            FileSize = fileSize,
            CreatedAt = DateTime.UtcNow,
            AutoDelete = deleteAfterDownload ?? false
        };

        var fileId = await _fileRepository.AddFileAsync(fileInfo);

        // Return the file ID or any other relevant information
        return fileId;
    }
        
    #endregion

}
