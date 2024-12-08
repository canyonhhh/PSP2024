using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.Models;

public class AppliedDiscountDTO
{
    public AppliedDiscountDTO(Guid discountId, DiscountMethod discountMethod, decimal amount, decimal percentage, decimal total, Guid businessId, Guid? productOrServiceGroupId, Guid? productOrServiceId)
    {
        DiscountId = discountId;
        DiscountMethod = discountMethod;
        Amount = amount;
        Percentage = percentage;
        Total = total;
        BusinessId = businessId;
        ProductOrServiceGroupId = productOrServiceGroupId;
        ProductOrServiceId = productOrServiceId;
    }

    [Required(ErrorMessage = "DiscountId is required")]
    public Guid DiscountId { get; set; }

    [Required(ErrorMessage = "DiscountMethod is required")]
    [EnumDataType(typeof(DiscountMethod), ErrorMessage = "Invalid DiscountMethod value")]
    public DiscountMethod DiscountMethod { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a non-negative value")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Percentage is required")]
    [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
    public decimal Percentage { get; set; }

    [Required(ErrorMessage = "Total is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Total must be a non-negative value")]
    public decimal Total { get; set; }

    [Required(ErrorMessage = "BusinessId is required")]
    public Guid BusinessId { get; set; }

    public Guid? ProductOrServiceGroupId { get; set; }
    public Guid? ProductOrServiceId { get; set; }
}
