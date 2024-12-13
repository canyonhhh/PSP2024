namespace PSPOS.ServiceDefaults.DTOs
{
    public class LoginResponseDto
    {
        public Guid BusinessId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}