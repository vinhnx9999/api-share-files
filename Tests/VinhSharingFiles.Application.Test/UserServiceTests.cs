using Moq;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Application.Services;
using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Application.Test;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _service = new UserService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsItems()
    {
        //Arrange
        var expectedItems = new List<User>
        {
            new() { Id = 1, DisplayName = "User 1", UserName = "user01", IsActive = true, Email= $"{Guid.NewGuid()}@demo.com" },
            new() { Id = 2, DisplayName = "User 2", UserName = "user02", IsActive = true, Email= $"{Guid.NewGuid()}@abc.com" }
        };

        _mockRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(expectedItems);

        //Act
        var result = await _service.GetAllUsersAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItems.Count, result.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsItem()
    {
        // Arrange
        var expectedItem = new User { Id = 1, DisplayName = "User 1", UserName = "user01", IsActive = true, Email = $"{Guid.NewGuid()}@demo.com" };
        _mockRepository.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(expectedItem);

        // Act
        var result = await _service.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItem.Id, result.Id);
    }

    [Fact]
    public async Task UpdateUserAsync_CallsRepositoryMethod()
    {
        // Arrange
        var updatedItem = new User {Id = 1, DisplayName = "User 1", UserName = "user01", IsActive = true, Email = $"{Guid.NewGuid()}@demo.com" };

        // Act
        await _service.UpdateUserAsync(updatedItem);

        // Assert
        _mockRepository.Verify(repo => repo.UpdateUserAsync(updatedItem), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_CallsRepositoryMethod()
    {
        // Arrange
        int itemIdToDelete = 1;

        // Act
        await _service.DeleteUserAsync(itemIdToDelete);

        // Assert
        _mockRepository.Verify(repo => repo.DeleteUserAsync(itemIdToDelete), Times.Once);
    }
}