using Microsoft.AspNet.Identity;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IEnumerable<UserInfoDto>> GetAllUsersAsync()
    {
        var data = await _userRepository.GetAllUsersAsync();

        return data.Select(user => new UserInfoDto
        {
            Id = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            IsActive = user.IsActive,
            CreatedDate = user.CreatedAt,
            ConfirmedDate = user.ConfirmedDate
        });
    }
        

    public async Task<UserInfoDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception("User not found");
        return new UserInfoDto
        {
            Id = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            IsActive = user.IsActive,
            CreatedDate = user.CreatedAt,
            ConfirmedDate = user.ConfirmedDate
        };
    }

    public async Task UpdateUserAsync(User user)
        => await _userRepository.UpdateUserAsync(user);

    public async Task DeleteUserAsync(int id)
        => await _userRepository.DeleteUserAsync(id);
}
