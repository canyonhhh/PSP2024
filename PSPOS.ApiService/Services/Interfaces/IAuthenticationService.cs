using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> AuthenticateAsync(LoginRequest request);
    string HashPin(string pin);
    string HashPassword(string password);
    bool VerifyPin(string plainTextPin, string hashedPin);
    bool VerifyPassword(string plainTextPassword, string hashedPassword);
    protected string GenerateJwtToken(User user);
}