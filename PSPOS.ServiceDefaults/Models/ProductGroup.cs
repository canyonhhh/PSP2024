namespace PSPOS.ServiceDefaults.Models;

public class ProductGroup : BaseClass
{
    public ProductGroup(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    public string Name { get; set; }
    public string? Description { get; set; }
}