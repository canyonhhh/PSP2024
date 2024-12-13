using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }
}