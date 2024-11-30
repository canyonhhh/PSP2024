using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services;
using PSPOS.ApiService.Services.Interfaces;
using LoginRequest = PSPOS.ServiceDefaults.Models.LoginRequest;

namespace PSPOS.ApiService.Controllers;

public class AuthController : Controller
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var response = await _authenticationService.AuthenticateAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
