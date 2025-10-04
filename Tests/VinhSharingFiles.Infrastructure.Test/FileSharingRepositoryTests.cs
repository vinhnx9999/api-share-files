using Microsoft.EntityFrameworkCore;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Infrastructure.Data;
using VinhSharingFiles.Infrastructure.Repositories;

namespace VinhSharingFiles.Infrastructure.Test;

public class FileSharingRepositoryTests
{
    private readonly VinhSharingDbContext _context;
    private readonly FileSharingRepository _repository;
    private readonly int userTestId = 1;

    public FileSharingRepositoryTests()
    {
        // Set up an in-memory database for testing
        var options = new DbContextOptionsBuilder<VinhSharingDbContext>()
            .UseInMemoryDatabase(databaseName: "SharingFileDb")
            .Options;

        _context = new VinhSharingDbContext(options);
        _repository = new FileSharingRepository(_context);
    }

    [Fact]
    public async Task AddItem_ShouldAddItemAsync()
    {
        //// Arrange
        //var newItem = new FileSharing
        //{
        //    Id = 1,
        //    DisplayName = $"file-{Guid.NewGuid()}-demo.txt",
        //    FileName = "STORE_TEXT_IN_DB",
        //    FileType = "STORE_TEXT_IN_DB",            
        //    AutoDelete = true,
        //    Description = "File demo description",
        //    UserId = userTestId,
        //    CreatedAt = DateTime.UtcNow
        //};

        //// Act
        //await _repository.AddFileAsync(newItem);

        //var items = await _repository.GetFileByIdAsync(1);

        //// Assert
        ////Assert.Single(items);
        //Assert.Equal(newItem.DisplayName, items.DisplayName);
    }

    [Fact]
    public async Task GetAllItems_ShouldReturnAllItemsAsync()
    {
        // Arrange
        var newItem01 = new FileSharing
        {
            Id = 1,
            DisplayName = $"file-{Guid.NewGuid()}.txt",
            FileName = "STORE_TEXT_IN_DB",
            AutoDelete = true,
            Description = "File demo description",
            UserId = userTestId,
            CreatedAt = DateTime.UtcNow
        };

        var newItem02 = new FileSharing
        {
            Id = 2,
            DisplayName = $"file-{Guid.NewGuid()}02.txt",
            FileName = "STORE_TEXT_IN_DB",
            AutoDelete = true,
            Description = "File demo description",
            UserId = userTestId,
            CreatedAt = DateTime.UtcNow
        };

        _context.FileSharings.AddRange(newItem01, newItem02);
        await _context.SaveChangesAsync();

        // Act
        var items = await _repository.GetAllFiles(userTestId);

        // Assert
        Assert.True(items.Count() >= 2);
    }

    [Fact]
    public async Task GetItemById_ShouldReturnItemAsync()
    {
        // Arrange
        var fileInfo = new FileSharing
        {
            Id = 5,
            DisplayName = $"file-{Guid.NewGuid()}05.txt",
            FileName = "STORE_TEXT_IN_DB",
            AutoDelete = true,
            Description = "File demo description",
            UserId = userTestId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.FileSharings.AddAsync(fileInfo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetFileByIdAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileInfo.FileName, result.FileName);
        Assert.Equal(fileInfo.DisplayName, result.DisplayName);
        Assert.Equal(fileInfo.Description, result.Description);
    }

    [Fact]
    public async Task DeleteItem_ShouldRemoveItemAsync()
    {
        // Arrange
        var fileInfo = new FileSharing
        {
            Id = 4,
            DisplayName = $"file-{Guid.NewGuid()}04.txt",
            FileName = "STORE_TEXT_IN_DB",
            AutoDelete = true,
            Description = "File demo description",
            UserId = userTestId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.FileSharings.AddAsync(fileInfo);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteFileByIdAsync(4);
        var result = await _repository.GetFileByIdAsync(4);

        // Assert
        Assert.Null(result);
    }
}