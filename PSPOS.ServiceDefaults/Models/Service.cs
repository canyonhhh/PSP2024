using Microsoft.VisualBasic;
using System.Xml.Linq;

namespace PSPOS.ServiceDefaults.Models;

public class Service : BaseClass
{
    public Service(string name, string? description, decimal price, DateInterval interval, Guid employeeId)
    {
        Name = name;
        Description = description;
        Price = price;
        Interval = interval;
        EmployeeId = employeeId;
    }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateInterval Interval { get; set; }
    public Guid EmployeeId { get; set; }
    public TimeSpan Duration { get; set; }
}