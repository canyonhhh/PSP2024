namespace PSPOS.ServiceDefaults.Models;

public class Tax : BaseClass
{
    public Tax(string name, string? description, bool active, decimal percentage, Guid businessId)
    {
        Name = name;
        Description = description;
        Active = active;
        Percentage = percentage;
        BusinessId = businessId;
    }

    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Active { get; set; }
    public decimal Percentage { get; set; }
    public Guid BusinessId { get; set; }
}