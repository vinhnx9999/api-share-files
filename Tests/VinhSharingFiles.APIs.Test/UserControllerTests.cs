using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VinhSharingFiles.APIs.Controllers;
using VinhSharingFiles.APIs.Models;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.DTOs;

namespace VinhSharingFiles.APIs.Test;

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

    [Fact]
    public async Task SignUp_ReturnsOkResult()
    {
        // Arrange
        var newItem = new SignUpModel
        {
            UserName = "newuser",
            Password = "P@ssw0rd",
            DisplayName = "New User",
            Email = "vinhnx9999@gmail.com"
        };

        // Act
        var result = await _controller.SignUp(newItem);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Equals(new { Message = "User created successfully" });            
    }

    [Fact]
    public async Task SignIn_ReturnsNotFound()
    {
        // Arrange
        var signInModel = new SignInModel { UserName = "newuser", Password = ""};

        // Act
        var result = await _controller.SignIn(signInModel);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task SignIn_ReturnsOkResult()
    {
        // Arrange
        var signInModel = new SignInModel { UserName = "newuser", Password = "P@ssw0rd" };
        var tokenInfo = new TokenInfoDto { DisplayName = "User demo", UserName = signInModel.UserName };
        _mockAuthService.Setup(service => service.SignInAsync(signInModel.UserName, signInModel.Password))
            .ReturnsAsync(tokenInfo);

        // Act
        var result = await _controller.SignIn(signInModel);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Equals(tokenInfo);
    }

    [Fact]
    public async Task Activate_By_Email_ReturnsOkResult()
    {
        // Arrange
        var newItem = new ActivateEmailModel
        {
            ActiveCode = Guid.NewGuid().ToString("N").ToUpper(),
            Email = "vinhnx9999@gmail.com"
        };

        // Act
        var result = await _controller.ActivateEmail(newItem);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Equals(new { Message = "Your account has been confirmed" });
    }

    [Fact]
    public async Task Activate_By_UserName_ReturnsOkResult()
    {
        // Arrange
        var newItem = new ActivateUserNameModel
        {
            ActiveCode = Guid.NewGuid().ToString("N").ToUpper(),
            UserName = "vinhnx9999"
        };

        // Act
        var result = await _controller.ActivateUserName(newItem);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Equals(new { Message = "Your account has been confirmed" });
    }

    [Fact]
    public async Task Forced_Activate_User_By_Id_ReturnsOkResult()
    {
        // Arrange
        int userId = 1;

        // Act
        var result = await _controller.ActivateUser(userId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Equals(new { Message = "Your account has been confirmed" });
    }
}