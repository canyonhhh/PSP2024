using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    public async Task<IActionResult> Login([FromForm] BlazorLoginRequest request)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        Console.WriteLine(client.BaseAddress);

        var response = await client.PostAsJsonAsync("Auth/login", new LoginRequestDto
        {
            Email = request.Email,
            Password = request.Password ?? string.Empty,
            Pin = request.Pin ?? string.Empty
        });

        if (!response.IsSuccessStatusCode)
            return Unauthorized("Invalid credentials");

        var authResponse = await response.Content.ReadFromJsonAsync<BlazorAuthResponse>();
        if (authResponse == null)
            return Unauthorized("Invalid response from authentication service");

        var claims = new List<Claim>
        {
            new Claim("BusinessId", authResponse.BusinessId.ToString()),
            new Claim(ClaimTypes.Role, authResponse.Role ?? "Employee")
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

public class BlazorLoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Pin { get; set; }
}

public class BlazorAuthResponse
{
    public Guid BusinessId { get; set; }
    public string Role { get; set; }
    public DateTime Expiration { get; set; }
}

public class LoginRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Pin { get; set; }
}