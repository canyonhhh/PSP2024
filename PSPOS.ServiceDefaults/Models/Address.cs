namespace PSPOS.ServiceDefaults.Models;

public class Address : BaseClass
{
    public Address(string street, string city, string? state, string? postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public string Street { get; set; }
    public string City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; }
}