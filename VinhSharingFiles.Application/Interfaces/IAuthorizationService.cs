using VinhSharingFiles.Domain.DTOs;

namespace VinhSharingFiles.Application.Interfaces
{
    public interface IAuthorizationService
    {
        Task ActivateEmailAsync(string email, string activeCode);
        Task ActivateUserNameAsync(string userName, string activeCode);
        Task RegisterUserAsync(string userName, string password, string displayName, string userEmail);
        Task<TokenInfoDto> SignInAsync(string userName, string password);
    }
}
