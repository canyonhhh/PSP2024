namespace PSPOS.ServiceDefaults.Models;

public class Order : BaseClass
{
    public Order(Guid businessId, Currency orderCurrency, decimal tip, OrderStatus status = OrderStatus.Open)
    {
        BusinessId = businessId;
        OrderCurrency = orderCurrency;
        Tip = tip;
        Status = status;
    }

    public Guid BusinessId { get; set; }
    public Currency OrderCurrency { get; set; }
    public decimal Tip { get; set; }
    public OrderStatus Status { get; set; }
}
