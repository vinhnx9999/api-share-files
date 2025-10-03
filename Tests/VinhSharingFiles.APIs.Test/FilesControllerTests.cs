using Microsoft.AspNetCore.Http;
using Moq;
using VinhSharingFiles.APIs.Controllers;
using VinhSharingFiles.Application.Interfaces;

namespace VinhSharingFiles.APIs.Test;

public class FilesControllerTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ICloudService> _mockService;
    private readonly FilesController _controller;

    public FilesControllerTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockService = new Mock<ICloudService>();

        _controller = new FilesController(
            _mockHttpContextAccessor.Object,
            _mockService.Object);
    }
}
