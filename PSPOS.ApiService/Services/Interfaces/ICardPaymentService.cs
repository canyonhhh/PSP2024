using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services.Interfaces;

public interface ICardPaymentService
{
    Task<string> ProcessPaymentAsync(Payment payment);
    Task<bool> RefundPaymentAsync(Payment refund);
}