using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VinhSharingFiles.APIs.Models;
using VinhSharingFiles.APIs.Utilities;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.APIs.Controllers;

public class FilesController(IHttpContextAccessor httpContextAccessor, IFileSharingService cloudService) : 
    BaseController(httpContextAccessor)
{
    private readonly IFileSharingService _cloudService = cloudService;       

    [HttpPost("{autoDelete}")]
    public async Task<IActionResult> UploadFileAsync(IFormFile file, bool? autoDelete)
    {
        int userId = GetUserId();
        var fileUploadedId = await _cloudService.UploadFileAsync(userId, file, autoDelete);
        string urlUploaded = IdEncryptor.EncryptId(fileUploadedId);

        return Ok(new
        {
            FileId = urlUploaded,
            DownloadUrl = $"api/Files/{urlUploaded}"
        });
    }

    [HttpPost("UploadString")]
    public async Task<IActionResult> UploadTextFileAsync([FromBody] TextFileModel model)
    {
        int userId = GetUserId();
        bool isSensitive = model.TextData.SensitiveKeywords();
        var fileUploadedId = await _cloudService.UploadTextFileAsync(userId, model.TextData, model.DeleteAfterAccessed, isSensitive);
        string urlUploaded = IdEncryptor.EncryptId(fileUploadedId);

        return Ok(new
        {
            FileId = urlUploaded,
            DownloadUrl = $"api/Files/{urlUploaded}"
        });
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFileByIdAsync(string id)
    {
        int fileId = IdEncryptor.DecryptId(id);
        int userId = GetUserId();

        var fileObj = userId > 0 ?
            await _cloudService.DownloadFileAsync(fileId) :
            await _cloudService.PreviewFileAsync(fileId);

        if (fileObj.ContentType == FileVariables.STORE_TEXT_IN_DB)
        {
            return Ok(new
            {
                FileId = id,
                Data = fileObj.Description,
            });
        }
        return File(fileObj.Data, fileObj.ContentType ?? "");
    }

    [HttpGet("{id}/Download")]
    public async Task<IActionResult> DownloadFileByIdAsync(string id)
    {
        int fileId = IdEncryptor.DecryptId(id);
        var fileObj = await _cloudService.DownloadFileAsync(fileId);
        if (fileObj.ContentType == FileVariables.STORE_TEXT_IN_DB)
        {
            return Ok(new
            {
                FileId = id,
                Data = fileObj.Description,
            });
        }

        return File(fileObj.Data, fileObj.ContentType ?? "");
    }
}
