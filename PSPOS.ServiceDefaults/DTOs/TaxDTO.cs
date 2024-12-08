using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.Models;

public class TaxDTO
{
    public TaxDTO(string name, string? description, bool active, decimal percentage, Guid businessId, Guid? productOrServiceGroupId, Guid? productOrServiceId)
    {
        Name = name;
        Description = description;
        Active = active;
        Percentage = percentage;
        BusinessId = businessId;
        ProductOrServiceGroupId = productOrServiceGroupId;
        ProductOrServiceId = productOrServiceId;
    }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Active is required")]
    public bool Active { get; set; }

    [Required(ErrorMessage = "Percentage is required")]
    [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
    public decimal Percentage { get; set; }

    [Required(ErrorMessage = "BusinessId is required")]
    public Guid BusinessId { get; set; }

    public Guid? ProductOrServiceGroupId { get; set; }
    public Guid? ProductOrServiceId { get; set; }
}