using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.Schemas;

namespace PSPOS.ApiService.Services.Interfaces;

public interface IOrderService
{
    // '/orders'
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<OrderSchema?> GetOrderSchemaByIdAsync(Guid id);
    Task<IEnumerable<OrderSchema>> GetAllOrdersAsync(string? status, int? limit, int? skip);
    Task<Order> AddOrderAsync(Guid businessId, string? status, string? currency);
    Task DeleteOrderAsync(Guid id);

    // '/orders/transactions'
    Task<IEnumerable<TransactionSchema>> GetAllTransactionsOfOrderAsync(Guid id);
    Task ProcessAllTransactionsOfOrderAsync(Guid id);
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task<TransactionSchema?> GetTransactionSchemaByIdAsync(Guid id);
    Task RefundTransactionAsync(Order order, Transaction transaction, RefundDTO refundDTO);

    // '/orders/items'
    Task<IEnumerable<OrderItemSchema>> GetAllItemsOfOrderAsync(Guid id);
    Task AddOrderItemToOrderAsync(OrderItemDTO orderItem);
    Task UpdateOrderItemAsync(Guid orderItemId, OrderItemDTO orderItem);
}
