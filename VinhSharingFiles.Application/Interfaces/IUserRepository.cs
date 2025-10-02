using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<User?> VerifyUserAsync(string userName, string password);
    }
}
