namespace PSPOS.ServiceDefaults.Models;

public class Service : BaseClass
{
    public Service(string name, string? description, decimal price, TimeSpan duration, Guid employeeId)
    {
        Name = name;
        Description = description;
        Price = price;
        Duration = duration;
        EmployeeId = employeeId;
    }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid EmployeeId { get; set; }
    public TimeSpan Duration { get; set; }
}