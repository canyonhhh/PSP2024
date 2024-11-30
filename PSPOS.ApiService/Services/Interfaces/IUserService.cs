using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync(string? role, string? name, string? surname, int limit, int skip);
    Task AddUserAsync(UserDTO userDto);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(Guid id);
    Task UpdateUserAsync(Guid user, UserDTO userDto);
    Task<User?> GetUserByIdAsync(Guid id);
}