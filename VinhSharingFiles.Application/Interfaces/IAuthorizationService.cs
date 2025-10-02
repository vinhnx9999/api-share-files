using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Application.Interfaces
{
    public interface IAuthorizationService
    {
        Task RegisterUserAsync(User user);
        Task<TokenInfoDto> SignInAsync(string userName, string password);
    }
}
