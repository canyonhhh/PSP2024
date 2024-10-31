namespace PSPOS.ServiceDefaults.Models;

public class Address(string street, string city, string? state, string? postalCode, string country)
    : BaseClass
{
    string Street { get; set; } = street;
    string City { get; set; } = city;
    string? State { get; set; } = state;
    string? PostalCode { get; set; } = postalCode;
    string Country { get; set; } = country;
}