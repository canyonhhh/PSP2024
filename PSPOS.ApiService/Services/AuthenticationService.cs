using Microsoft.IdentityModel.Tokens;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PSPOS.ApiService.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse> AuthenticateAsync(LoginRequestDto requestDTO)
    {
        var user = await _userRepository.GetByEmailAsync(requestDTO.Email);

        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Validate based on role
        if (user.Role == UserRole.Employee)
        {
            if (string.IsNullOrEmpty(requestDTO.Pin))
                throw new UnauthorizedAccessException("PIN is required");

            if (!BCrypt.Net.BCrypt.Verify(requestDTO.Pin, user.PinHash))
                throw new UnauthorizedAccessException("Invalid PIN");
        }
        else
        {
            if (string.IsNullOrEmpty(requestDTO.Password))
                throw new UnauthorizedAccessException("Password is required");

            if (!BCrypt.Net.BCrypt.Verify(requestDTO.Password, user.PasswordHash))
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
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key is missing"));
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