using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto requestDTO);
    string HashPassword(string password);
    bool VerifyPassword(string plainTextPassword, string hashedPassword);
    string GenerateJwtToken(User user);
}