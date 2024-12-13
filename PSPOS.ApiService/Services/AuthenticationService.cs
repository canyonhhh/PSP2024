using Microsoft.IdentityModel.Tokens;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PSPOS.ApiService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto requestDTO)
        {
            var user = await _userRepository.GetByEmailAsync(requestDTO.Email);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            if (string.IsNullOrEmpty(requestDTO.Password))
                throw new UnauthorizedAccessException("Password is required");

            if (!BCrypt.Net.BCrypt.Verify(requestDTO.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid password");

            var token = GenerateJwtToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiration = jwtToken.ValidTo;

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var businessIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value;
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserRole")?.Value;

            if (string.IsNullOrEmpty(businessIdClaim) || string.IsNullOrEmpty(roleClaim))
                throw new Exception("Token missing required claims.");

            if (!Guid.TryParse(businessIdClaim, out var businessId))
                throw new Exception("Invalid BusinessId in token.");

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new Exception("Invalid UserId in token.");

            return new LoginResponseDto
            {
                BusinessId = businessId,
                UserId = userId,
                Role = roleClaim,
                Expiration = expiration,
                Token = token
            };
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string plainTextPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key is missing"));


            Console.WriteLine($"User ID: {user.Id}");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserRole", user.Role.ToString()),
                    new Claim("BusinessId", user.BusinessId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}