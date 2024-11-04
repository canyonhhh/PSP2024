using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services;

public class UserService
{
    private readonly UserRepository _userRepository;
    
    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }
    
    public async Task<User> AddUserAsync(User user)
    {
        return await _userRepository.AddUserAsync(user);
    }
}