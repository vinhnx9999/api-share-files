using Microsoft.EntityFrameworkCore;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Infrastructure.Data;
using VinhSharingFiles.Infrastructure.Repositories;

namespace VinhSharingFiles.Infrastructure.Test;

public class UserRepositoryTests
{
    private readonly VinhSharingDbContext _context;
    private readonly UserRepository _repository;
    private readonly string displayName = "User Test 01";

    public UserRepositoryTests()
    {
        // Set up an in-memory database for testing
        var options = new DbContextOptionsBuilder<VinhSharingDbContext>()
            .UseInMemoryDatabase(databaseName: "SharingFileDb")
            .Options;

        _context = new VinhSharingDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddItem_ShouldAddItemAsync()
    {
        // Arrange
        var newItem = new User
        {
            Id = 1,
            DisplayName = displayName,
            UserName = "userTest01",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.AddUserAsync(newItem);
        var items = await _repository.GetUserByIdAsync(1);

        // Assert
        //Assert.Single(items);
        Assert.Equal(displayName, items.DisplayName);
    }

    [Fact]
    public async Task GetAllItems_ShouldReturnAllItemsAsync()
    {
        // Arrange
        var newItem01 = new User
        {
            Id = 1,
            ActiveCode = "SomeActiveCode",
            Email = $"{Guid.NewGuid()}@demo.com",
            Password = "SomePassword",
            IsActive = true,
            DisplayName = displayName,
            UserName = "userTest01",
            CreatedAt = DateTime.UtcNow
        };

        var newItem02 = new User
        {
            Id = 2,
            ActiveCode = "SomeActiveCode",
            Email = $"{Guid.NewGuid()}@demo.com",
            Password = "SomePassword",
            IsActive = true,
            DisplayName = displayName.Replace("01", "02"),
            UserName = "userTest02",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.AddRange(newItem01, newItem02);
        await _context.SaveChangesAsync();

        // Act
        var items = await _repository.GetAllUsersAsync();

        // Assert
        Assert.True(items.Count() >= 2);
    }

    [Fact]
    public async Task GetItemById_ShouldReturnItemAsync()
    {
        // Arrange
        var userInfo = new User
        {
            Id = 5,
            DisplayName = displayName.Replace("01", "05"),
            UserName = "userTest05",
            ActiveCode = "SomeActiveCode",
            Email = $"{Guid.NewGuid()}@demo.com",
            Password = "SomePassword",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(userInfo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByIdAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userInfo.UserName, result.UserName);
        Assert.Equal(userInfo.DisplayName, result.DisplayName);
    }

    [Fact]
    public async Task DeleteItem_ShouldRemoveItemAsync()
    {
        // Arrange
        var userInfo = new User
        {
            Id = 4,
            DisplayName = displayName.Replace("01", "04"),
            UserName = "userTest04",
            ActiveCode = "SomeActiveCode",
            Email = $"{Guid.NewGuid()}@demo.com",
            Password = "SomePassword",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(userInfo);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteUserAsync(4);
        var result = await _repository.GetUserByIdAsync(4);

        // Assert
        Assert.Null(result);
    }
}