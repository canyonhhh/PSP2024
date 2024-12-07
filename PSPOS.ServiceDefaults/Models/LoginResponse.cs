namespace PSPOS.ServiceDefaults.Models;

public class LoginResponse
{
    public LoginResponse(string token, DateTime expiration)
    {
        Token = token;
        Expiration = expiration;
    }

    public LoginResponse()
    {
        throw new NotImplementedException();
    }

    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}
