namespace PSPOS.ServiceDefaults.Models;

public class AppliedDiscount : BaseClass
{
    public AppliedDiscount(DiscountMethod method, decimal amount, decimal percentage, Guid discountId, Guid orderItemId, Guid orderId)
    {
        Method = method;
        Amount = amount;
        Percentage = percentage;
        DiscountId = discountId;
        OrderItemId = orderItemId;
        OrderId = orderId;
    }

    public DiscountMethod Method { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public Guid DiscountId { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid OrderId { get; set; }
}