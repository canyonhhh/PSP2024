using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task AddOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        var order = await GetOrderByIdAsync(id);
        if(order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsOfOrderAsync(Guid orderId)
    {
        // Ensure the order exists
        var order = await GetOrderByIdAsync(orderId) ?? throw new ArgumentException($"Order with ID {orderId} does not exist.");

        // Retrieve all OrderItems associated with the order
        var orderItemTransactionIds = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .Select(oi => oi.TransactionId)
            .ToListAsync();

        // Retrieve Transactions by their IDs
        var transactions = await _context.Transactions
            .Where(t => orderItemTransactionIds.Contains(t.Id))
            .ToListAsync();

        return transactions;
    }

    public async Task ProcessAllTransactionsOfOrderAsync(Guid id)
    {
        var order = await GetOrderByIdAsync(id);
        throw new NotImplementedException();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    public async Task RefundTransactionByIdAsync(Guid id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<OrderItem>> GetAllItemsOfOrderAsync(Guid orderId)
    {
        // Ensure the order exists
        var order = await GetOrderByIdAsync(orderId) ?? throw new ArgumentException($"Order with ID {orderId} does not exist.");

        // Retrieve all OrderItems associated with the order
        var orderItems = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();

        return orderItems;
    }

    public async Task AddOrderItemToOrderAsync(OrderItem orderItem)
    {
        // Ensure the order exists
        var order = await GetOrderByIdAsync(orderItem.OrderId) ?? throw new ArgumentException($"Order with ID {orderItem.OrderId} does not exist.");

        await _context.OrderItems.AddAsync(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrderItemOfOrderAsync(OrderItem orderItem)
    {
        // Ensure the order exists
        var order = await GetOrderByIdAsync(orderItem.OrderId) ?? throw new ArgumentException($"Order with ID {orderItem.OrderId} does not exist.");

        _context.OrderItems.Update(orderItem);
        await _context.SaveChangesAsync();
    }
}
