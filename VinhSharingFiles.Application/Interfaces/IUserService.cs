using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserInfoDto>> GetAllUsersAsync();
    Task<UserInfoDto> GetUserByIdAsync(int id);        
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
}
