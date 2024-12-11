using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using Stripe;

namespace PSPOS.ApiService.Services
{
    public class StripeService : ICardPaymentService
    {

        private readonly IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<string> ProcessPaymentAsync(Payment payment)
        {
            var options = new PaymentIntentCreateOptions
            {
                // convert payment.amount to cents from decimal
                Amount = (long)payment.Amount * 100,
                Currency = payment.PaymentCurrency.ToString().ToLower(),
                PaymentMethod = payment.ExternalPaymentId.ToString(),
                ConfirmationMethod = "automatic",
                Confirm = true,
                Description = payment.TransactionId.ToString(),
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return paymentIntent.Id;

        }

        public async Task<bool> RefundPaymentAsync(Payment refund)
        {
            var options = new RefundCreateOptions
            {
                Amount = (long)refund.Amount * 100,
                PaymentIntent = refund.TransactionId.ToString(),
            };

            var service = new RefundService();
            var refundResponse = await service.CreateAsync(options);

            return refundResponse.Status == "succeeded";
        }
    }
}