namespace PSPOS.ServiceDefaults.Models;

public class User : BaseClass
{
    public User()
    {
    }

    public User(string firstName, string lastName, string email, string phone, UserRole role, Guid businessId, string passwordHash)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Role = role;
        BusinessId = businessId;
        PasswordHash = passwordHash;
    }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required UserRole Role { get; set; }
    public required Guid BusinessId { get; set; }
    public required string PasswordHash { get; set; }
}