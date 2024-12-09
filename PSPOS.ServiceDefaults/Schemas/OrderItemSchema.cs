using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ServiceDefaults.Schemas
{
    public class OrderItemSchema : BaseClass
    {
        public string? type { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public Guid orderId { get; set; }
        public Guid serviceId { get; set; }
        public Guid productId { get; set; }
        public Guid transactionId { get; set; }
    }
}