using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.DTOs;

namespace PSPOS.ApiService.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
        var key = Encoding.ASCII.GetBytes("e56ccfdac2091df4a0377cd7db900c23b716f8830cc178f6401cea7a12b42a6f3389edb104a6703036f5a56093c422e289b6292176659c45786b7493cca4371e0b20ea931768aac892f7f83084074c51c63f975d46020810a93943ed1d48454b56ad4d265a2dde18fea260ad117ad78fa8cc48088265cdcbf240b9f5e20a358646be1c022699e81147c3be18115a8d335904d00513c232c7dca6a32dfc1e3bcdee59c6ecbcbf3a57b7c2622cfca7266eb17e012079223855911a25d474a9c93a50ea17ae9f5bb2143eac7fef1cc49ef5044e59ad8901f4fee0382a286fc9587090f55fe72d428fbe43d541f24d07c23c5353a5929482b92d00e2f17b4c452238"); // TODO: Replace with env variable
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
