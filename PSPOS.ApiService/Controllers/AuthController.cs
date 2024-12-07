using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;

namespace PSPOS.ApiService.Controllers;

public class AuthController : Controller
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto requestDTO)
    {
        try
        {
            var response = await _authenticationService.AuthenticateAsync(requestDTO);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = response.Expiration
            };

            Response.Cookies.Append("AuthToken", response.Token, cookieOptions);

            return Ok(new { message = "Login successful" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
