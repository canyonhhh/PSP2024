using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(string? role, string? name, string? surname, int limit, int skip)
    {
        return await _userRepository.GetAllUsersAsync(role, name, surname, limit, skip);
    }

    public async Task AddUserAsync(UserDTO userDto)
    {
        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            Phone = userDto.Phone,
            Role = (UserRole)userDto.Role,
            BusinessId = userDto.BusinessId
        };

        await _userRepository.AddUserAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateUserAsync(user);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await _userRepository.DeleteUserAsync(id);
    }

    public async Task UpdateUserAsync(Guid user, UserDTO userDto) // used to update user from the API
    {
        var userToUpdate = new User
        {
            Id = user,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            Phone = userDto.Phone,
            Role = (UserRole)userDto.Role,
            BusinessId = userDto.BusinessId
        };
        
        // update the user
        await _userRepository.UpdateUserAsync(userToUpdate);
    }
}