using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Processors
{
    public class GiftCardPaymentProcessor : IPaymentProcessor
    {
        public Task<string?> ProcessPaymentAsync(Payment payment)
        {
            // TODO
        }

        public Task<bool> RefundPaymentAsync(Payment payment)
        {
            // TODO
        }
    }
}