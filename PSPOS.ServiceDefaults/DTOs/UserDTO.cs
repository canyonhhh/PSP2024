namespace PSPOS.ServiceDefaults.DTOs;

public class UserDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Role { get; set; }
    public string? Password { get; set; } // Plaintext password for owners
    public string? Pin { get; set; } // Plaintext PIN for employees
    public Guid BusinessId { get; set; }
}
