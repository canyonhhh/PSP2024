using AutoMapper;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IAuthenticationService authenticationService, IMapper mapper)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(string? role, string? name, string? surname, int limit, int skip, string? businessId)
    {
        return await _userRepository.GetAllUsersAsync(role, name, surname, limit, skip, businessId);
    }

    public async Task AddUserAsync(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);

        if (!string.IsNullOrWhiteSpace(userDto.Password))
        {
            user.PasswordHash = _authenticationService.HashPassword(userDto.Password);
        }

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

    public async Task UpdateUserAsync(Guid user, UserDto userDto) // used to update user from the API
    {
        var userToUpdate = _mapper.Map<User>(userDto);

        // update the user
        await _userRepository.UpdateUserAsync(userToUpdate);
    }
}