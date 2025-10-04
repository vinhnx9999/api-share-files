using Microsoft.EntityFrameworkCore;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Infrastructure.Data;

namespace VinhSharingFiles.Infrastructure.Repositories;

public class UserRepository(VinhSharingDbContext context) : IUserRepository
{
    private readonly VinhSharingDbContext _context = context;

    public async Task<IEnumerable<User>> GetAllUsersAsync()
        => await _context.Users.ToListAsync();


    public async Task<User?> GetUserByIdAsync(int id) => await _context.Users.FindAsync(id);

    public async Task<bool> AddUserAsync(User user)
    {
        try
        {
            var users = await _context.Users
                .Where(x => x.UserName == user.UserName)
                .ToListAsync();

            if (users.Count != 0)
                return false;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }            
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> VerifyUserAsync(string userName, string password)
    {
        var users = await _context.Users
            .Where(x => x.UserName == userName && x.IsActive)
            .ToListAsync();

       return users.FirstOrDefault(x => x.Password == password);
    }

    public async Task ActivateUserByIdAsync(int userId)
    {
        var user = await _context.Users
            .Where(x => x.Id == userId && !x.IsActive)
            .FirstOrDefaultAsync();

        if (user != null)
        {
            user.IsActive = true;
            user.ActiveCode = string.Empty;
            user.ConfirmedDate = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ActivateEmailAsync(string email, string activeCode)
    {
        try
        {
            await ActivateEmailInternalAsync(email, activeCode);
            return true;
        }
        catch
        {
            return false;
        }                
    }

    public async Task<bool> ActivateUserNameAsync(string userName, string activeCode)
    {
        try
        {
            await ActivateUserNameInternalAsync(userName, activeCode);
            return true;
        }
        catch
        {
            return false;
        }
            
    }

    private async Task ActivateEmailInternalAsync(string email, string activeCode)
    {
        var users = await _context.Users
             .Where(x => x.Email == email && !x.IsActive)
             .ToListAsync();

        var user = users.FirstOrDefault(x => x.ActiveCode == activeCode);
        if (user != null)
        {
            user.IsActive = true;
            user.ActiveCode = string.Empty;
            user.ConfirmedDate = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }

    private async Task ActivateUserNameInternalAsync(string userName, string activeCode)
    {
        var users = await _context.Users
            .Where(x => x.UserName == userName && !x.IsActive)
            .ToListAsync();

        var user = users.FirstOrDefault(x => x.ActiveCode == activeCode);
        if (user != null)
        {
            user.IsActive = true;
            user.ActiveCode = string.Empty;
            user.ConfirmedDate = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
