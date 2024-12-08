namespace PSPOS.ServiceDefaults.Models;

public class Service : BaseClass
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public TimeSpan Duration { get; set; }
}