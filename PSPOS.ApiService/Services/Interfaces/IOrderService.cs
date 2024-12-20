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
    Task<Order> AddOrderAsync(Guid businessId, string? status, string? currency, Guid createdBy);
    Task DeleteOrderAsync(Guid id);
    Task<Order?> UpdateOrderAsync(Guid orderId, OrderDTO orderDTO);

    // '/orders/transactions'
    Task<IEnumerable<TransactionSchema>> GetAllTransactionsOfOrderAsync(Guid id);
    Task ProcessTransactionForOrderAsync(Guid id, TransactionDTO transactionDTO);
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task<TransactionSchema?> GetTransactionSchemaByIdAsync(Order orderId, Guid transactionId);
    Task RefundTransactionAsync(Order order, TransactionSchema transactionSchema, RefundDTO refundDTO);
    Task<String?> GetExternalPaymentIdAsync(Guid orderId, Guid transactionId);

    // '/orders/items'
    Task<IEnumerable<OrderItemSchema>> GetAllItemsOfOrderAsync(Guid id);
    Task AddOrderItemToOrderAsync(Guid orderId, OrderItemDTO orderItem);
    Task UpdateOrderItemAsync(Guid orderItemId, OrderItemDTO orderItem);
}