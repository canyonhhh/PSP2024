using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories.Interfaces;

public interface IOrderRepository
{
    // '/orders'
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task AddOrderAsync(Order order);
    Task DeleteOrderAsync(Guid id);

    // '/orders/transactions'
    Task<IEnumerable<Transaction>> GetAllTransactionsOfOrderAsync(Guid id);
    Task ProcessAllTransactionsOfOrderAsync(Guid id);
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task RefundTransactionByIdAsync(Guid id);

    // '/orders/items'
    Task<IEnumerable<OrderItem>> GetAllItemsOfOrderAsync(Guid id);
    Task AddOrderItemToOrderAsync(OrderItem orderItem);
    Task UpdateOrderItemOfOrderAsync(OrderItem orderItem);
}
