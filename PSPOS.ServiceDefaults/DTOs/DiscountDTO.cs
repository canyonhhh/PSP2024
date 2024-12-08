using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.Models;

public class DiscountDTO
{
    public DiscountDTO(string name, string? description, DiscountMethod method, bool active, decimal? amount, decimal? percentage, DateTime endDate, Guid businessId, Guid? productOrServiceGroupId, Guid? productOrServiceId)
    {
        Name = name;
        Description = description;
        Method = method;
        Active = active;
        Amount = amount;
        Percentage = percentage;
        EndDate = endDate;
        BusinessId = businessId;
        ProductOrServiceGroupId = productOrServiceGroupId;
        ProductOrServiceId = productOrServiceId;
    }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Method is required")]
    public DiscountMethod Method { get; set; }

    [Required(ErrorMessage = "Active is required")]
    public bool Active { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value")]
    public decimal? Amount { get; set; }

    [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
    public decimal? Percentage { get; set; }

    [Required(ErrorMessage = "EndDate is required")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "BusinessId is required")]
    public Guid BusinessId { get; set; }

    public Guid? ProductOrServiceGroupId { get; set; }
    public Guid? ProductOrServiceId { get; set; }
}
