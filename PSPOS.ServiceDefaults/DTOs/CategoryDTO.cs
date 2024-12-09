using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.Models;

public class CategoryDTO
{
    public CategoryDTO(string name, string? description, Guid[] productOrServiceIds)
    {
        Name = name;
        Description = description;
        ProductOrServiceIds = productOrServiceIds;
    }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "ProductOrServiceIds are required")]
    public Guid[] ProductOrServiceIds { get; set; }
}
