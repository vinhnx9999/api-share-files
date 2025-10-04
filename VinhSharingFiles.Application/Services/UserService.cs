using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IEnumerable<UserInfoDto>> GetAllUsersAsync() 
        => await _userRepository.GetAllUsersAsync();

    public async Task<UserInfoDto> GetUserByIdAsync(int id)
        => await _userRepository.GetUserByIdAsync(id);

    public async Task UpdateUserAsync(User user)
        => await _userRepository.UpdateUserAsync(user);

    public async Task DeleteUserAsync(int id)
        => await _userRepository.DeleteUserAsync(id);
}
