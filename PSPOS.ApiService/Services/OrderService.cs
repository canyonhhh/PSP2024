using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _orderRepository.GetOrderByIdAsync(id);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? status, int? limit, int? skip)
    {
        return await _orderRepository.GetAllOrdersAsync(status, limit, skip);
    }

    public async Task<Order> AddOrderAsync(Guid businessId, string status, string currency)
    {
        return await _orderRepository.AddOrderAsync(businessId, status, currency);
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        await _orderRepository.DeleteOrderAsync(id);
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsOfOrderAsync(Guid id)
    {
        return await _orderRepository.GetAllTransactionsOfOrderAsync(id);
    }

    public async Task ProcessAllTransactionsOfOrderAsync(Guid id)
    {
        await _orderRepository.ProcessAllTransactionsOfOrderAsync(id);
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        return await _orderRepository.GetTransactionByIdAsync(id);
    }

    public async Task RefundTransactionAsync(Transaction transaction)
    {
        await _orderRepository.RefundTransactionAsync(transaction);
    }

    public async Task<IEnumerable<OrderItem>> GetAllItemsOfOrderAsync(Guid id)
    {
        return await _orderRepository.GetAllItemsOfOrderAsync(id);
    }

    public async Task AddOrderItemToOrderAsync(OrderItem orderItem)
    {
        await _orderRepository.AddOrderItemToOrderAsync(orderItem);
    }

    public async Task UpdateOrderItemAsync(OrderItem orderItem)
    {
        await _orderRepository.UpdateOrderItemAsync(orderItem);
    }
}
