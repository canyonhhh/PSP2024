using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.Schemas;

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
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? status, int? limit, int? skip)
    {
        var query = _context.Orders.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                query = query.Where(o => o.Status == parsedStatus);
            else
                throw new ArgumentException($"Status '{status}' does not exist."); // Catch in controller
        }

        if (skip != null)
            query = query.Skip((int)skip);

        if (limit != null)
            query = query.Take((int)limit);

        return await query.ToListAsync();
    }

    public async Task<Order> AddOrderAsync(Guid businessId, string? status, string? currency)
    {
        Currency currencyEnum = 0; // Default if null
        OrderStatus statusEnum = 0; // Default if null

        if (!string.IsNullOrEmpty(status))
        {
            if (!Enum.TryParse(status, true, out statusEnum))
                throw new ArgumentException($"Status '{status}' does not exist."); // Catch in controller
        }

        if (!string.IsNullOrEmpty(currency))
        {
            if (!Enum.TryParse(currency, true, out currencyEnum))
                throw new ArgumentException($"Currency '{currency}' does not exist."); // Catch in controller
        }

        var order = new Order(businessId, currencyEnum, 0, statusEnum);

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task DeleteOrderAsync(Guid orderId)
    {
        var order = await GetOrderByIdAsync(orderId);
        if (order != null)
        {
            // Remove related OrderItems
            var orderItems = _context.OrderItems.Where(oi => oi.OrderId == orderId);
            _context.OrderItems.RemoveRange(orderItems);

            // Remove the Order itself
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsOfOrderAsync(Guid orderId)
    {
        // Ensure the order exists
        var order = await GetOrderByIdAsync(orderId) ?? throw new ArgumentException($"Order with ID '{orderId}' does not exist.");

        // Retrieve all transaction IDs associated with the order
        var orderItemTransactionIds = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .Select(oi => oi.TransactionId)
            .Distinct()
            .ToListAsync();

        // Retrieve Transactions by their IDs
        var transactions = await _context.Transactions
            .Where(t => orderItemTransactionIds.Contains(t.Id))
            .ToListAsync();

        return transactions;
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId)
    {
        return await _context.Transactions.FindAsync(transactionId);
    }

    public async Task<Giftcard?> GetGiftCardByCode(string giftcardCode)
    {
        if (string.IsNullOrWhiteSpace(giftcardCode))
            throw new ArgumentException("Gift card code cannot be null or empty.", nameof(giftcardCode));

        // Query to find the gift card object with the matching code
        var giftCard = await _context.GiftCards
            .FirstOrDefaultAsync(gc => gc.Code == giftcardCode);

        return giftCard; // Returns null if no matching gift card is found
    }

    public async Task UpdateGiftCardAmountAsync(Giftcard giftcard)
    {
        if (giftcard == null)
            throw new ArgumentNullException(nameof(giftcard), "Gift card object cannot be null.");

        // Find the existing gift card in the database by its ID or Code
        var existingGiftCard = await _context.GiftCards
            .FirstOrDefaultAsync(gc => gc.Code == giftcard.Code);

        if (existingGiftCard == null)
            throw new InvalidOperationException("Gift card not found.");

        // Update the gift card's amount
        existingGiftCard.Amount = giftcard.Amount;

        // Save the changes to the database
        _context.GiftCards.Update(existingGiftCard);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsOfTransactionAsync(Guid transactionId)
    {
        return await _context.Payments.Where(p => p.TransactionId == transactionId).ToArrayAsync();
    }

    public async Task<IEnumerable<OrderItem>> GetAllItemsOfOrderAsync(Guid orderId)
    {
        // Ensure the order exists
        var order = await GetOrderByIdAsync(orderId) ?? throw new ArgumentException($"Order with ID '{orderId}' does not exist.");

        // Retrieve all OrderItems associated with the order
        var orderItems = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();

        return orderItems;
    }

    public async Task AddOrderItemToOrderAsync(OrderItem orderItem)
    {
        // Ensure the order exists
        if ((await GetOrderByIdAsync(orderItem.OrderId)) == null)
            throw new ArgumentException($"Order with ID '{orderItem.OrderId}' does not exist.");

        await _context.OrderItems.AddAsync(orderItem);
        await _context.SaveChangesAsync();
    }
    public async Task<OrderItem?> GetOrderItemByIdAsync(Guid orderItemId)
    {
        return await _context.OrderItems.FindAsync(orderItemId);
    }
    public async Task UpdateOrderItemAsync(OrderItem orderItem)
    {
        if ((await GetOrderItemByIdAsync(orderItem.Id)) == null)
            throw new ArgumentException($"Order item with ID '{orderItem.Id}' does not exist.");

        _context.OrderItems.Update(orderItem);
        await _context.SaveChangesAsync();
    }
}