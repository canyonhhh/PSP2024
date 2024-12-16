using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.Schemas;
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
        public List<AppliedDiscountSchema>? appliedDiscounts { get; set; } = new List<AppliedDiscountSchema>();
        public List<AppliedTaxSchema>? appliedTaxes { get; set; } = new List<AppliedTaxSchema>();
    }
}