namespace PSPOS.ServiceDefaults.Models;

public class Product : BaseClass
{
    public Product(string name, string? description, decimal? price, string? imageUrl, Guid businessId, Guid? baseProductId)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        BusinessId = businessId;
        BaseProductId = baseProductId;
    }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public Guid BusinessId { get; set; }
    public Guid? BaseProductId { get; set; }
}