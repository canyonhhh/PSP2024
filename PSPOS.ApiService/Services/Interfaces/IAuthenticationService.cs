using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.DTOs;

namespace PSPOS.ApiService.Services.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> AuthenticateAsync(LoginRequestDto requestDTO);
    string HashPin(string pin);
    string HashPassword(string password);
    bool VerifyPin(string plainTextPin, string hashedPin);
    bool VerifyPassword(string plainTextPassword, string hashedPassword);
    protected string GenerateJwtToken(User user);
}
