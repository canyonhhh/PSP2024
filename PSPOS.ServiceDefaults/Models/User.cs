namespace PSPOS.ServiceDefaults.Models;

public class User(string firstName, string lastName, string email, string phone, UserRole role, Guid businessId)
    : BaseClass
{
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public string Email { get; set; } = email;
    public string Phone { get; set; } = phone;
    public UserRole Role { get; set; } = role;

    public Guid BusinessId { get; set; } = businessId;
}