using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using VinhSharingFiles.APIs.Controllers;
using VinhSharingFiles.APIs.Models;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Application.DTOs;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.APIs.Test;

public class FilesControllerTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IFileSharingService> _mockService;
    private readonly FilesController _controller;

    public FilesControllerTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockService = new Mock<IFileSharingService>();

        _controller = new FilesController(
            _mockHttpContextAccessor.Object,
            _mockService.Object);
    }

    [Fact]
    public async Task DownloadFileById_ReturnsTextResultAsync()
    {
        // Arrange
        string fileId = "2rPwjW0379";

        var newItem = new FileObjectDto { FileId = 3, Description = "New Task", ContentType = FileVariables.STORE_TEXT_IN_DB };
        var testContent = "This is some test content for the file.";
        var testBytes = System.Text.Encoding.UTF8.GetBytes(testContent);
        var memoryStream = new MemoryStream(testBytes);
        
        _mockService.Setup(service => service.DownloadFileAsync(newItem.FileId ?? 3))
            .ReturnsAsync(new FileObjectDto
            {
                FileId = newItem.FileId,
                Data = memoryStream,
                ContentType = newItem.ContentType,
                Description = newItem.Description
            });

        // Act
        var result = await _controller.DownloadFileByIdAsync(fileId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Equals(new { FileId = fileId, Data = newItem.Description });        
    }

    [Fact]
    public async Task UploadTextFile_ReturnsOkResultAsync()
    {
        // Arrange
        int userId = 1; 
        TextFileModel model = new()
        {
            TextData = "This is a sample text data.",
            DeleteAfterAccessed = true
        };

        _mockService.Setup(service => service.UploadTextFileAsync(userId, model.TextData, model.DeleteAfterAccessed, false)).ReturnsAsync(4);

        // Mock the claims data
        var claims = new List<Claim>
        {
            new("id", $"{userId}"),
            new(ClaimTypes.Name, "testuser"),
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Role, "admin")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        // Set the mock user on the controller's HttpContext
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = principal
        });

        // Act
        var result = await _controller.UploadTextFileAsync(model);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UploadFile_ReturnsOkResultAsync()
    {
        // Arrange
        int userId = 1;
        bool deleteAfterAccessed = true;
        var content = "Hello World from a Fake File";
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        _mockService.Setup(service => service.UploadFileAsync(userId, file, deleteAfterAccessed)).ReturnsAsync(4);

        // Mock the claims data
        var claims = new List<Claim>
        {
            new("id", $"{userId}"),
            new(ClaimTypes.Name, "testuser"),
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Role, "admin")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        // Set the mock user on the controller's HttpContext
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = principal
        });

        // Act
        var result = await _controller.UploadFileAsync(file, deleteAfterAccessed);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
