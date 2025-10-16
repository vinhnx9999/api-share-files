using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VinhSharingFiles.APIs.Utilities;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Application.DTOs;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.APIs.Controllers;

public class DocumentsController(IHttpContextAccessor httpContextAccessor, IFileSharingService cloudService) :
    BaseController(httpContextAccessor)
{
    private readonly IFileSharingService _cloudService = cloudService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FileObjectDto>>> GetAllFiles()
    {
        int userId = GetUserId();
        var files = await _cloudService.GetAllFilesByUserIdAsync(userId);

        return Ok(files);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFileByIdAsync(string id)
    {
        int fileId = IdEncryptor.DecryptId(id);
        var fileObj = await _cloudService.PreviewFileAsync(fileId);
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
