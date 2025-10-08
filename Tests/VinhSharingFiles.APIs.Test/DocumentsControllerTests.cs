using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using VinhSharingFiles.APIs.Controllers;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.APIs.Test;

public class DocumentsControllerTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IFileSharingService> _mockService;
    private readonly DocumentsController _controller;

    public DocumentsControllerTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockService = new Mock<IFileSharingService>();

        _controller = new DocumentsController(
            _mockHttpContextAccessor.Object,
            _mockService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithItemsAsync()
    {
        // Arrange
        int userId = 1;
        var items = new List<FileDto>
        {
            new() { Id = 1, FileName = "FileName 1", ContentType = "text/html" },
            new() { Id = 2, FileName = "FileName 2", ContentType = "application/json" }
        };

        _mockService.Setup(service => service.GetAllFilesByUserIdAsync(userId)).ReturnsAsync(items);
        
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
        var result = await _controller.GetAllFiles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItems = Assert.IsType<List<FileDto>>(okResult.Value);
        Assert.Equal(2, returnedItems.Count);
    }

    [Fact]
    public async Task PreviewFileById_ReturnsTextResultAsync()
    {
        // Arrange
        string fileId = "2rPwjW0379";

        var newItem = new FileObjectDto { FileId = 3, Description = "New Task", ContentType = FileVariables.STORE_TEXT_IN_DB };
        var testContent = "This is some test content for the file.";
        var testBytes = System.Text.Encoding.UTF8.GetBytes(testContent);
        var memoryStream = new MemoryStream(testBytes);

        _mockService.Setup(service => service.PreviewFileAsync(newItem.FileId ?? 3))
            .ReturnsAsync(new FileObjectDto
            {
                FileId = newItem.FileId,
                Data = memoryStream,
                ContentType = newItem.ContentType,
                Description = newItem.Description
            });

        // Act
        var result = await _controller.GetFileByIdAsync(fileId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Equals(new { FileId = fileId, Data = newItem.Description });
    }

}
