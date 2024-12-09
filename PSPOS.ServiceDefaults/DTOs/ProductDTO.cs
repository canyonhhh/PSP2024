using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.Models;

public class ProductDTO
{
    public ProductDTO(string name, string? description, decimal? price, string? imageUrl, int stockQuantity, Guid businessId, Guid? baseProductId)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        StockQuantity = stockQuantity;
        BusinessId = businessId;
        BaseProductId = baseProductId;
    }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
    public decimal? Price { get; set; }

    [Url(ErrorMessage = "ImageUrl must be a valid URL")]
    public string? ImageUrl { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "StockQuantity must be a non-negative value")]
    public int StockQuantity { get; set; }

    [Required(ErrorMessage = "BusinessId is required")]
    public Guid BusinessId { get; set; }

    public Guid? BaseProductId { get; set; }
}
