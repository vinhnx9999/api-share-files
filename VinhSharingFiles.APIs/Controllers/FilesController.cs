using Microsoft.AspNetCore.Mvc;
using VinhSharingFiles.APIs.Models;
using VinhSharingFiles.APIs.Utilities;
using VinhSharingFiles.Application.Interfaces;

namespace VinhSharingFiles.APIs.Controllers
{
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

        [HttpPost("UploadString/{autoDelete}")]
        public async Task<IActionResult> UploadString2FileAsync([FromBody] string textInput, bool? autoDelete)
        {
            int userId = GetUserId();
            var fileUploadedId = await _cloudService.UploadTextFileAsync(userId, textInput, autoDelete);
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
        public async Task<IActionResult> GetFileByIdAsync(string id)
        {
            int fileId = IdEncryptor.DecryptId(id);
            var fileObj = await _cloudService.DownloadFileAsync(fileId);
            return File(fileObj.Data, fileObj.ContentType ?? "");
        }

        //[HttpDelete]
        //public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
        //{
        //    var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        //    if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist");
        //    await _s3Client.DeleteObjectAsync(bucketName, key);
        //    return NoContent();
        //}
    }
}
