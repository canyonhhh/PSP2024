using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories.Interfaces;

public interface IOrderRepository
{
    // '/orders'
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task UpdateOrder(Order order);
    Task<IEnumerable<Order>> GetAllOrdersAsync(string? status, int? limit, int? skip);
    Task<Order> AddOrderAsync(Guid businessId, string? status, string? currency);
    Task DeleteOrderAsync(Guid id);

    // '/orders/transactions'
    Task<IEnumerable<Transaction>> GetAllTransactionsOfOrderAsync(Guid id);
    Task AddPaymentAsync(Payment payment);
    Task AddTransactionAsync(Transaction transaction);
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task<IEnumerable<Payment>> GetAllPaymentsOfTransacionAsync(Guid transactionId);
    Task<Giftcard?> GetGiftcardByIdAsync(Guid id);

    // '/orders/items'
    Task<IEnumerable<OrderItem>> GetAllItemsOfOrderAsync(Guid id);
    Task AddOrderItemToOrderAsync(OrderItem orderItem);
    Task UpdateOrderItemAsync(OrderItem orderItem);
}