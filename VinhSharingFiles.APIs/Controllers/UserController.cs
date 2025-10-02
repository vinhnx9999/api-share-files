using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VinhSharingFiles.APIs.Models;
using VinhSharingFiles.Domain.DTOs;
using IAuthorizationService = VinhSharingFiles.Application.Interfaces.IAuthorizationService;
using IUserService = VinhSharingFiles.Application.Interfaces.IUserService;

namespace VinhSharingFiles.APIs.Controllers
{
    public class UserController(
        IHttpContextAccessor httpContextAccessor, 
        IAuthorizationService authService,
        IUserService userService) : 
        BaseController(httpContextAccessor)
    {
        private readonly IAuthorizationService _authService = authService;
        private readonly IUserService _userService = userService;

        private readonly string DEFAULT_PASSWORD = "P@ssw0rd";

        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] SignUpModel model)
        {
            string passWord = model.Password ?? DEFAULT_PASSWORD;

            if (passWord.Length < 5) passWord = DEFAULT_PASSWORD;
            await _authService.RegisterUserAsync(model.UserName,
                passWord, 
                model.DisplayName ?? String.Empty, 
                model.Email ?? String.Empty);

            return Ok(new { Message = "User created successfully" });
        }

        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInModel model)
        {
            var userInfo = await _authService.SignInAsync(model.UserName, model.Password);
            return Ok(userInfo);
        }

        [HttpPost("activate-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ActivateEmail([FromBody] ActivateEmailModel model)
        {
            await _authService.ActivateEmailAsync(model.Email, model.ActiveCode);
            return Ok(new { Message = "Your account has been confirmed" });
        }

        [HttpPost("activate-username")]
        [AllowAnonymous]
        public async Task<IActionResult> ActivateUserName([FromBody] ActivateUserNameModel model)
        {
            await _authService.ActivateUserNameAsync(model.UserName, model.ActiveCode);

            return Ok(new { Message = "Your account has been confirmed" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfoDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetAllUser()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }
    }
}
