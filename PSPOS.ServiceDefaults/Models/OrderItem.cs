namespace PSPOS.ServiceDefaults.Models;

public class OrderItem : BaseClass
{
    public OrderItem(OrderItemType type, decimal price, int quantity, Guid orderId, Guid serviceId, Guid productId, Guid transactionId)
    {
        Type = type;
        Price = price;
        Quantity = quantity;
        OrderId = orderId;
        ServiceId = serviceId;
        ProductId = productId;
        TransactionId = transactionId;
    }

    public OrderItemType Type { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Guid OrderId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid ProductId { get; set; }
    public Guid TransactionId { get; set; }
}