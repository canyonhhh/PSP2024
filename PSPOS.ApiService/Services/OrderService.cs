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

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllOrdersAsync();
    }

    public async Task AddOrderAsync(Order order)
    {
        await _orderRepository.AddOrderAsync(order);
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

    public async Task RefundTransactionByIdAsync(Guid id)
    {
        await _orderRepository.RefundTransactionByIdAsync(id);
    }

    public async Task<IEnumerable<OrderItem>> GetAllItemsOfOrderAsync(Guid id)
    {
        return await _orderRepository.GetAllItemsOfOrderAsync(id);
    }

    public async Task AddOrderItemToOrderAsync(OrderItem orderItem)
    {
        await _orderRepository.AddOrderItemToOrderAsync(orderItem);
    }

    public async Task UpdateOrderItemOfOrderAsync(OrderItem orderItem)
    {
        await _orderRepository.UpdateOrderItemOfOrderAsync(orderItem);
    }
}
