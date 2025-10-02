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
    public class AuthorizationService(IConfiguration configuration, IUserRepository userRepository) : IAuthorizationService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly SymmetricSecurityKey _tokenSecurityKey = new(System.Text.Encoding.ASCII.GetBytes($"{configuration["TokenSecurityKey"]}"));

        public async Task RegisterUserAsync(User user) 
            => await _userRepository.AddUserAsync(user);

        public async Task<TokenInfoDto> SignInAsync(string userName, string password)
        {
            var user = await _userRepository.VerifyUserAsync(userName, password);
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
