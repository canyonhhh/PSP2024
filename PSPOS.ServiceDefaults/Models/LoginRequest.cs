namespace PSPOS.ServiceDefaults.Models;

public class LoginRequest
{
		public string Email { get; set; }
		public string? Pin { get; set; }
		public string? Password { get; set; }
}
