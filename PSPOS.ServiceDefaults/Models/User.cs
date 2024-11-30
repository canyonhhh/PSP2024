namespace PSPOS.ServiceDefaults.Models;

public class User : BaseClass
{
    public User(string firstName, string lastName, string email, string phone, UserRole role, Guid businessId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Role = role;
        BusinessId = businessId;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public UserRole Role { get; set; }
    public Guid BusinessId { get; set; }
    public string? PinHash { get; set; }
    public string? PasswordHash { get; set; }
}