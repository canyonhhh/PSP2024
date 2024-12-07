using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<IEnumerable<User>> GetAllUsersAsync(string? role, string? name, string? surname, int limit, int skip)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(u => u.Role.ToString() == role);
        }

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(u => u.FirstName.Contains(name) || u.LastName.Contains(name));
        }

        if (!string.IsNullOrEmpty(surname))
        {
            query = query.Where(u => u.LastName.Contains(surname));
        }

        return Task.FromResult<IEnumerable<User>>(query.Skip(skip).Take(limit).ToList());
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await GetUserByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}