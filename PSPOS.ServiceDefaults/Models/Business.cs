using System.Runtime.InteropServices.JavaScript;

namespace PSPOS.ServiceDefaults.Models;

public class Business(string name, string phone, string email, Currency defaultCurrency = Currency.Eur)
    : BaseClass
{
    public string Name { get; set; } = name;
    public string Phone { get; set; } = phone;
    public string Email { get; set; } = email;
    public Currency DefaultCurrency { get; set; } = defaultCurrency;
}