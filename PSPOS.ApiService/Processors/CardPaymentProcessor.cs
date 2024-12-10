using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Processors
{
    public class CardPaymentProcessor : IPaymentProcessor
    {
        public async Task<string?> ProcessPaymentAsync(Payment payment)
        {
            // TODO
        }

        public async Task<bool> RefundPaymentAsync(Payment payment)
        {
            // TODO
        }
    }
}