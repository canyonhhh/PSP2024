using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public required string Email { get; set; }

    public string? Pin { get; set; }

    public string? Password { get; set; }
}