namespace PSPOS.ServiceDefaults.Models;

public class ProductGroup : BaseClass
{
    public ProductGroup(string name, string? description, Guid[]? productOrServiceIds)
    {
        Name = name;
        Description = description;
        this.productOrServiceIds = productOrServiceIds ?? Array.Empty<Guid>(); // Use input or empty array
    }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid[]? productOrServiceIds { get; set; }
}