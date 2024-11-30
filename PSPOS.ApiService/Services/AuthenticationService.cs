using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Validate based on role
        if (user.Role == UserRole.Employee)
        {
            if (string.IsNullOrEmpty(request.Pin))
                throw new UnauthorizedAccessException("PIN is required");

            if (!BCrypt.Net.BCrypt.Verify(request.Pin, user.PinHash))
                throw new UnauthorizedAccessException("Invalid PIN");
        }
        else
        {
            if (string.IsNullOrEmpty(request.Password))
                throw new UnauthorizedAccessException("Password is required");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid password");
        }

        // Generate a JWT token
        var token = GenerateJwtToken(user);
        
        return new LoginResponse(token, DateTime.UtcNow.AddHours(1));
    }

    public string HashPin(string pin)
    {
        return BCrypt.Net.BCrypt.HashPassword(pin);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPin(string plainTextPin, string hashedPin)
    {
        return BCrypt.Net.BCrypt.Verify(plainTextPin, hashedPin);
    }

    public bool VerifyPassword(string plainTextPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("YourSecretKeyHere"); // TODO: Replace with a secure key
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("BusinessId", user.BusinessId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
