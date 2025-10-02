using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VinhSharingFiles.APIs.Controllers;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.DTOs;

namespace VinhSharingFiles.APIs.Test
{
    public class UserControllerTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IAuthorizationService> _mockAuthService;
        private readonly Mock<IUserService> _mockUserService;

        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockAuthService = new Mock<IAuthorizationService>();
            _mockUserService = new Mock<IUserService>();

            _controller = new UserController(
                _mockHttpContextAccessor.Object, 
                _mockAuthService.Object, 
                _mockUserService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithItems()
        {
            // Arrange
            var items = new List<UserInfoDto>
            {
                new UserInfoDto { Id = 1, DisplayName = "User 1" },
                new UserInfoDto { Id = 2, DisplayName = "User 2" }
            };

            _mockUserService.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(items);

            // Act
            var result = await _controller.GetAllUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedItems = Assert.IsType<List<UserInfoDto>>(okResult.Value);
            Assert.Equal(2, returnedItems.Count);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            _mockUserService.Setup(service => service.GetUserByIdAsync(1)).ReturnsAsync((UserInfoDto)null);

            // Act
            var result = await _controller.GetUser(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithItem()
        {
            // Arrange
            var item = new UserInfoDto { Id = 1, DisplayName = "User 1" };
            _mockUserService.Setup(service => service.GetUserByIdAsync(1)).ReturnsAsync(item);

            // Act
            var result = await _controller.GetUser(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedItem = Assert.IsType<UserInfoDto>(okResult.Value);
            Assert.Equal(item.Id, returnedItem.Id);
        }
    }
}