using Microsoft.AspNetCore.Mvc;
using VinhSharingFiles.APIs.Models;
using VinhSharingFiles.APIs.Utilities;
using VinhSharingFiles.Application.Interfaces;

namespace VinhSharingFiles.APIs.Controllers;

public class FilesController(IHttpContextAccessor httpContextAccessor, ICloudService cloudService) : 
    BaseController(httpContextAccessor)
{
    private readonly ICloudService _cloudService = cloudService;       

    [HttpPost("{autoDelete}")]
    public async Task<IActionResult> UploadFileAsync(IFormFile file, bool? autoDelete)
    {
        int userId = GetUserId();
        var fileUploadedId = await _cloudService.UploadFileAsync(userId, file, autoDelete);
        string urlUploaded = IdEncryptor.EncryptId(fileUploadedId);
        var fileUploaded = new
        {
            FileId = urlUploaded,
            DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/Files/{urlUploaded}"
        };
        return Ok(fileUploaded);
    }

    [HttpPost("UploadString")]
    public async Task<IActionResult> UploadTextFileAsync([FromBody] TextFileModel model)
    {
        int userId = GetUserId();
        var fileUploadedId = await _cloudService.UploadTextFileAsync(userId, model.TextData, model.DeleteAfterAccessed);
        string urlUploaded = IdEncryptor.EncryptId(fileUploadedId);
        var fileUploaded = new
        {
            FileId = urlUploaded,
            DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/Files/{urlUploaded}"
        };
        return Ok(fileUploaded);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadFileByIdAsync(string id)
    {
        int fileId = IdEncryptor.DecryptId(id);
        var fileObj = await _cloudService.DownloadFileAsync(fileId);
        if (fileObj.ContentType == "STORE_TEXT_IN_DB")
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
