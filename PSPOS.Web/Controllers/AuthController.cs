using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PSPOS.ServiceDefaults.DTOs;
using System.Security.Claims;

namespace PSPOS.BlazorApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public AuthController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequestDto request)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("Auth/login", request);
            if (!response.IsSuccessStatusCode)
                return Unauthorized("Invalid credentials");

            var authResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (authResponse == null)
                return Unauthorized("Invalid response from authentication service");

            var claims = new List<Claim>
            {
                new Claim("BusinessId", authResponse.BusinessId.ToString()),
                new Claim("UserId", authResponse.UserId.ToString()),
                new Claim("UserRole", authResponse.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = authResponse.Expiration
            });

            return Redirect("/");
        }
    }
}