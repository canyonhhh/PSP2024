namespace PSPOS.ServiceDefaults.Models;

public class ServiceGroup : BaseClass
{
    public ServiceGroup(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    public string Name { get; set; }
    public string? Description { get; set; }
}