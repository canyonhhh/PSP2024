using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var authToken = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

        if (string.IsNullOrWhiteSpace(authToken))
        {
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
        var user = new ClaimsPrincipal(identity);

        return Task.FromResult(new AuthenticationState(user));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(AddPadding(payload));
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
        }

        return claims;
    }

    private static string AddPadding(string input)
    {
        return input.PadRight(input.Length + (4 - input.Length % 4) % 4, '=');
    }
}
