using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Processors
{
    public interface IPaymentProcessor
    {
        Task<string?> ProcessPaymentAsync(Payment payment);
        Task<bool> RefundPaymentAsync(Payment payment);
    }
}