using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.Schemas;

namespace PSPOS.ApiService.Repositories.Interfaces;

public interface IOrderRepository
{
    // '/orders'
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task UpdateOrder(Order order);
    Task<IEnumerable<Order>> GetAllOrdersAsync(string? status, int? limit, int? skip);
    Task<Order> AddOrderAsync(Guid businessId, string? status, string? currency, Guid createdBy);
    Task DeleteOrderAsync(Guid id);

    // '/orders/transactions'
    Task<IEnumerable<Transaction>> GetAllTransactionsOfOrderAsync(Guid id);
    Task AddPaymentAsync(Payment payment);
    Task AddTransactionAsync(Transaction transaction);
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task<IEnumerable<Payment>> GetAllPaymentsOfTransactionAsync(Guid transactionId);

    // '/orders/items'
    Task<IEnumerable<OrderItemSchema>> GetAllItemsOfOrderAsync(Guid id);
    Task<IEnumerable<OrderItem>> GetAllItemsOfOrderAsyncO(Guid orderId);
    Task AddOrderItemToOrderAsync(OrderItem orderItem);
    Task<OrderItem?> GetOrderItemByIdAsync(Guid orderItemId);
    Task UpdateOrderItemAsync(OrderItem orderItem);
}