using Microsoft.Extensions.Configuration;
using Moq;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Application.Services;
using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.Application.Test;

public class AmazonS3ServiceTests
{
    private readonly Mock<IFileSharingRepository> _mockRepository;
    private readonly AmazonS3Service _service;

    public AmazonS3ServiceTests()
    {
        _mockRepository = new Mock<IFileSharingRepository>();
        var inMemorySettings = new Dictionary<string, string>
        {
            {"MySettingKey", "InMemoryValue"},
            {"AWS:AccessKey", "AccessKey"},
            {"AWS:SecretKey", "SecretKey"},
            {"AWS:Region", "Region"},
            {"AWS:BucketName", "BucketName"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _service = new AmazonS3Service(configuration, _mockRepository.Object);
    }

    [Fact]
    public async Task GetAllFilesByUserId_ReturnsItemsAsync()
    {
        //Arrange
        var expectedItems = new List<FileSharing>
        {
            new() { Id = 1, FileName = "FileName 1", FileType="text/plain", Description = "Description 1" },
            new() { Id = 2, FileName = "FileName 2", FileType="text/plain", Description = "Description 2" }
        };
        int userId = 1;

        _mockRepository.Setup(repo => repo.GetAllFiles(userId)).ReturnsAsync(expectedItems);

        //Act
        var result = await _service.GetAllFilesByUserIdAsync(userId);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(expectedItems.Count(), result.Count());
    }

    [Fact]
    public async Task PreviewFile_ReturnsItemAsync()
    {
        // Arrange
        var expectedItem = new FileSharing { Id = 1, FileName = FileVariables.STORE_TEXT_IN_DB, Description = "Description 1" };
        _mockRepository.Setup(repo => repo.GetFileByIdAsync(1)).ReturnsAsync(expectedItem);

        // Act
        var result = await _service.PreviewFileAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItem.FileName, result.Name);
    }

    [Fact]
    public async Task DownloadFile_ReturnsItemAsync()
    {
        // Arrange
        var expectedItem = new FileSharing { Id = 1, FileName = FileVariables.STORE_TEXT_IN_DB, Description = "Description 1" };
        _mockRepository.Setup(repo => repo.GetFileByIdAsync(1)).ReturnsAsync(expectedItem);

        // Act
        var result = await _service.DownloadFileAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItem.FileName, result.Name);
    }
}