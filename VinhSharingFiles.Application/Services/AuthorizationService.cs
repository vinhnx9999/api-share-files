using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.Application.Services
{
    public class AuthorizationService(
        IConfiguration configuration, 
        IUserRepository userRepository) : IAuthorizationService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly string _encryptKey = $"{configuration["EncryptKey:AesKey"]}";
        private readonly string _encryptSalt = $"{configuration["EncryptKey:Salt"]}";
        private readonly SymmetricSecurityKey _tokenSecurityKey = new(System.Text.Encoding.ASCII.GetBytes($"{configuration["TokenSecurityKey"]}"));

        public async Task RegisterUserAsync(User user)
            => await _userRepository.AddUserAsync(user);

        public async Task RegisterUserAsync(string userName, string password, string displayName, string userEmail)
        {
            EncryptionService encryptionService = new(_encryptKey, _encryptSalt);
            string encryptedText = encryptionService.Encrypt(password);
            string activeCode = Guid.NewGuid().ToString("N").ToUpper();

            var user = new User
            {
                UserName = userName,
                Password = encryptedText,
                DisplayName = displayName,
                Email = userEmail,
                ActiveCode = activeCode,
                CreatedAt = DateTime.UtcNow
            };

            bool isCreated = await _userRepository.AddUserAsync(user);

            if (!isCreated)
                throw new InvalidOperationException("User creation failed. The username or email might already exist.");

            EmailSender emailSender = new(configuration);
            emailSender.SendEmail(userEmail, "Welcome to SharingFiles", $"Your account has been created. Username: {userName} and ActiveCode: {activeCode}");
        }

        public async Task ActivateEmailAsync(string email, string activeCode)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(activeCode))
                throw new ArgumentException("Active code is required.", nameof(activeCode));

            bool isActivated = await _userRepository.ActivateEmailAsync(email, activeCode);

            if (!isActivated)
                throw new InvalidOperationException("Activation failed. Please check your email and active code.");

            EmailSender emailSender = new(configuration);
            emailSender.SendEmail(email, "Account Activated", "Your email has been successfully activated.");
        }
                  

        public async Task ActivateUserNameAsync(string userName, string activeCode)
        {
            bool isActivated = await _userRepository.ActivateUserNameAsync(userName, activeCode);

            if (!isActivated)
                throw new InvalidOperationException("Activation failed. Please check your username and active code.");
        }

        public async Task<TokenInfoDto> SignInAsync(string userName, string password)
        {
            EncryptionService encryptionService = new(_encryptKey, _encryptSalt);
            string encryptedText = encryptionService.Encrypt(password);

            var user = await _userRepository.VerifyUserAsync(userName, encryptedText);
            var expiredTime = DateTime.UtcNow.AddMinutes(SecurityClaims.MaximumExpirationTime);

            return user == null
                ? throw new UnauthorizedAccessException("Invalid username or password.")
                : new TokenInfoDto
                {
                    AccessToken = IssueCertified(user, expiredTime),
                    DisplayName = user.DisplayName ?? "",
                    ExpirationTime = 60,
                    ExpiredAt = expiredTime,
                    UserName = user.UserName ?? ""
                };
        }

        private string IssueCertified(User user, DateTime expiredTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName ?? ""),
                new("scopes", SecurityClaims.Scopes),
                new("aud", $"Audience {Guid.NewGuid()}"),
                new("iss", $"Internal Token"),
                new(ClaimTypes.Expired, $"{expiredTime.Ticks}"),
                new(ClaimTypes.Sid, $"{user.Id:X2}"),
                new(ClaimTypes.NameIdentifier, $"{user.DisplayName}"),
                new(ClaimTypes.System, SecurityClaims.System)
            };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ExternalBearer);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = expiredTime,
                SigningCredentials = new SigningCredentials(_tokenSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}