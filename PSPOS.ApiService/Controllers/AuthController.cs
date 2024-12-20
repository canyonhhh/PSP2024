using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using Serilog;

namespace PSPOS.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            Log.Information("Login attempt for email: {Email}", requestDTO.Email);
            try
            {
                var response = await _authenticationService.AuthenticateAsync(requestDTO);
                Log.Information("Login successful for email: {Email}", requestDTO.Email);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("Unauthorized access attempt for email: {Email}. Error: {Message}", requestDTO.Email, ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred during login attempt for email: {Email}. Error: {Message}", requestDTO.Email, ex.Message);
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }
    }
}