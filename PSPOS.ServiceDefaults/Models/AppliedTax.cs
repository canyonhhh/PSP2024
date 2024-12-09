namespace PSPOS.ServiceDefaults.Models;

public class AppliedTax : BaseClass
{
    public AppliedTax(decimal percentage, Guid taxId, Guid orderItemId, Guid orderId)
    {
        Percentage = percentage;
        TaxId = taxId;
        OrderItemId = orderItemId;
        OrderId = orderId;
    }

    public decimal Percentage { get; set; }
    public Guid TaxId { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid OrderId { get; set; }
}