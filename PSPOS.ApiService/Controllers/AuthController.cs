using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PSPOS.ApiService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDTO)
    {
        try
        {
            var response = await _authenticationService.AuthenticateAsync(requestDTO);

            var (businessId, role) = ExtractClaimsFromToken(response.Token);

            return Ok(new
            {
                BusinessId = businessId,
                Role = role,
                response.Expiration
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    private (Guid? BusinessId, string? Role) ExtractClaimsFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
        {
            throw new SecurityTokenException("Invalid token");
        }

        var jwtToken = handler.ReadJwtToken(token);

        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var businessIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value;

        Guid? businessId = null;
        if (Guid.TryParse(businessIdClaim, out var parsedBusinessId))
        {
            businessId = parsedBusinessId;
        }

        return (businessId, roleClaim);
    }
}