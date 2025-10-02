using Microsoft.EntityFrameworkCore;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Infrastructure.Data;
using VinhSharingFiles.Infrastructure.Repositories;

namespace VinhSharingFiles.Infrastructure.Test
{
    public class UserRepositoryTests
    {
        private readonly VinhSharingDbContext _context;
        private readonly UserRepository _repository;
        private string displayName = "User Test 01";

        public UserRepositoryTests()
        {
            // Set up an in-memory database for testing
            var options = new DbContextOptionsBuilder<VinhSharingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new VinhSharingDbContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddItemAsync_ShouldAddItem()
        {
            // Arrange
            var newItem = new User
            {
                Id = 1,
                DisplayName = displayName,
                UserName = "userTest01",
                CreatedAt = DateTime.Now
            };

            // Act
            await _repository.AddUserAsync(newItem);
            var items = await _repository.GetUserByIdAsync(1);

            // Assert
            //Assert.Single(items);
            Assert.Equal(displayName, items.DisplayName);
        }

        [Fact]
        public async Task GetAllItemsAsync_ShouldReturnAllItems()
        {
            // Arrange
            _context.Users.AddRange(
                new User
                {
                    Id = 2,
                    DisplayName = displayName.Replace("01", "02"),
                    UserName = "userTest02",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Id = 3,
                    DisplayName = displayName.Replace("01", "03"),
                    UserName = "userTest03",
                    CreatedAt = DateTime.Now
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var items = await _repository.GetAllUsersAsync();

            // Assert
            Assert.True(items.Count() >= 2);
        }

        [Fact]
        public async Task GetItemByIdAsync_ShouldReturnItem()
        {
            // Arrange
            var item = new User
            {
                Id = 5,
                DisplayName = displayName.Replace("01", "05"),
                UserName = "userTest05",
                CreatedAt = DateTime.Now
            };

            await _context.Users.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByIdAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(displayName, result.DisplayName);
        }

        [Fact]
        public async Task DeleteItemAsync_ShouldRemoveItem()
        {
            // Arrange
            var item = new User
            {
                Id = 4,
                DisplayName = displayName.Replace("01", "04"),
                UserName = "userTest04",
                CreatedAt = DateTime.Now
            };

            await _context.Users.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteUserAsync(4);
            var result = await _repository.GetUserByIdAsync(4);

            // Assert
            Assert.Null(result);
        }

    }
}