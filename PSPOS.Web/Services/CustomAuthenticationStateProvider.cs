using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

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
            // No token found, return anonymous user
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            return Task.FromResult(new AuthenticationState(anonymousUser));
        }

        try
        {
            // Parse the JWT token and set claims
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
        catch
        {
            // If parsing fails, return anonymous user
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            return Task.FromResult(new AuthenticationState(anonymousUser));
        }
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(AddPadding(payload));
        var keyValuePairs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if(keyValuePairs != null)
        {
            foreach (var kvp in keyValuePairs)
            {
                claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
            }
        }

        return claims;
    }

    private static string AddPadding(string input)
    {
        return input.PadRight(input.Length + (4 - input.Length % 4) % 4, '=');
    }
}
