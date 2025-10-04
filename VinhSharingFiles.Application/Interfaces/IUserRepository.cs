using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<UserInfoDto>> GetAllUsersAsync();
    Task<UserInfoDto> GetUserByIdAsync(int id);
    Task<bool> AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task<User?> VerifyUserAsync(string userName, string password);
    Task<bool> ActivateEmailAsync(string email, string activeCode);
    Task<bool> ActivateUserNameAsync(string userName, string activeCode);
    Task ActivateUserByIdAsync(int userId);
}
