using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.DTOs;

public class UserDto
{
    public UserDto(string firstName, string lastName, string email, string phone, int role, Guid businessId, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Role = role;
        BusinessId = businessId;
        Password = password;
    }

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public int Role { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Business ID is required")]
    public Guid BusinessId { get; set; }
}