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

    public async Task<Order> AddOrderAsync(Guid businessId, string? status, string? currency, Guid createdBy)
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

        var order = new Order(businessId, createdBy, currencyEnum, 0, statusEnum);

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
    public async Task<IEnumerable<OrderItem>> GetAllItemsOfOrderAsyncO(Guid orderId)
    {
        // Ensure the order exists
        var order = await GetOrderByIdAsync(orderId) ?? throw new ArgumentException($"Order with ID '{orderId}' does not exist.");

        // Retrieve all OrderItems associated with the order
        var orderItems = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();

        return orderItems;
    }
    public async Task<IEnumerable<OrderItemSchema>> GetAllItemsOfOrderAsync(Guid orderId)
    {
        // Step 1: Ensure the order exists
        var orderExists = await _context.Orders.AnyAsync(o => o.Id == orderId);
        if (!orderExists)
            throw new ArgumentException($"Order with ID '{orderId}' does not exist.");

        // Step 2: Fetch all OrderItems for the given order ID
        var orderItems = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();

        if (!orderItems.Any())
            return Enumerable.Empty<OrderItemSchema>();

        // Step 3: Fetch AppliedDiscounts and AppliedTaxes
        var appliedDiscounts = await _context.AppliedDiscounts
            .Where(ad => ad.OrderId == orderId)
            .ToListAsync();

        var appliedTaxes = await _context.AppliedTax
            .Where(at => at.OrderId == orderId)
            .ToListAsync();

        // Step 4: Map OrderItems to OrderItemSchema with null handling
        var orderItemSchemas = orderItems.Select(item =>
{
    var discounts = appliedDiscounts
        ?.Where(ad => ad.OrderItemId == item.Id)
        .Select(ad => new AppliedDiscountSchema
        {
            Id = ad.Id,
            amount = ad.Amount,
            percentage = ad.Percentage,
            discountId = ad.DiscountId,
            orderItemId = ad.OrderItemId,
            orderId = ad.OrderId
        })
        .ToList() ?? new List<AppliedDiscountSchema>();

    var taxes = appliedTaxes
        ?.Where(at => at.OrderItemId == item.Id)
        .Select(at => new AppliedTaxSchema
        {
            Id = at.Id,
            percentage = at.Percentage,
            taxId = at.TaxId,
            orderItemId = at.OrderItemId,
            orderId = at.OrderId
        })
        .ToList() ?? new List<AppliedTaxSchema>();

    // Return the mapped schema
    return new OrderItemSchema
    {
        Id = item.Id,
        price = item.Price,
        quantity = item.Quantity,
        type = item.Type.ToString(), 
        orderId = item.OrderId,
        serviceId = item.ServiceId,
        productId = item.ProductId,
        transactionId = item.TransactionId,
        appliedDiscounts = discounts, // Default empty list if null
        appliedTaxes = taxes           // Default empty list if null
    };
}).ToList();

        return orderItemSchemas;
    }

    public async Task AddOrderItemToOrderAsync(OrderItem orderItem)
    {
        // Step 1: Validate the order exists
        var order = await GetOrderByIdAsync(orderItem.OrderId);
        if (order == null)
            throw new ArgumentException($"Order with ID '{orderItem.OrderId}' does not exist.");

        // Step 2: Add the OrderItem
        await _context.OrderItems.AddAsync(orderItem);
        await _context.SaveChangesAsync();

        // Step 3: Fetch product group (categories) mapping
        var productGroups = await _context.ProductGroups.ToListAsync();
        var productGroupDict = new Dictionary<Guid, Guid>();

        foreach (var group in productGroups)
        {
            foreach (var productId in group.productOrServiceIds ?? Array.Empty<Guid>())
            {
                if (!productGroupDict.ContainsKey(productId))
                {
                    productGroupDict[productId] = group.Id;
                }
            }
        }

        // Step 4: Determine applicable category for the product
        if (!productGroupDict.TryGetValue(orderItem.ProductId, out var productGroupId))
        {
            // No category found; return early
            return;
        }

        // Step 5: Fetch and apply discounts
        var discount = await _context.Discounts
            .Where(d => d.Active && d.EndDate > DateTime.UtcNow && d.ProductOrServiceGroupId == productGroupId)
            .OrderByDescending(d => d.Amount) // Prioritize the largest discount
            .FirstOrDefaultAsync();

        if (discount != null)
        {
            decimal discountAmount = 0;

            // Calculate the discount amount
            if (string.Equals(discount.Method, "FIXED", StringComparison.OrdinalIgnoreCase))
            {
                discountAmount = Math.Min(discount.Amount, orderItem.Price * orderItem.Quantity);
            }
            else if (string.Equals(discount.Method, "PERCENTAGE", StringComparison.OrdinalIgnoreCase))
            {
                discountAmount = (orderItem.Price * orderItem.Quantity) * (discount.Percentage / 100);
            }

            // Create an AppliedDiscount record
            var appliedDiscount = new AppliedDiscount(
                method: discount.Method == "FIXED" ? DiscountMethod.Fixed : DiscountMethod.PercentageFromTotal,
                amount: discountAmount,
                percentage: discount.Percentage,
                discountId: discount.Id,
                orderItemId: orderItem.Id,
                orderId: orderItem.OrderId
            );

            await _context.AppliedDiscounts.AddAsync(appliedDiscount);
        }

        // Step 6: Fetch and apply taxes
        var tax = await _context.Taxes
            .Where(t => t.ProductOrServiceGroupId == productGroupId)
            .FirstOrDefaultAsync();

        if (tax != null)
        {
            decimal taxAmount = (orderItem.Price * orderItem.Quantity) * ((decimal)tax.Percentage / 100);

            // Create an AppliedTax record
            var appliedTax = new AppliedTax(
                percentage: (decimal)tax.Percentage,
                taxId: tax.Id,
                orderItemId: orderItem.Id,
                orderId: orderItem.OrderId
            );

            await _context.AppliedTax.AddAsync(appliedTax);
        }
        // Step 7: Save all changes (OrderItem, AppliedDiscount, AppliedTax)
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
    public async Task UpdateOrderAsync(Order order)
    {
        if (!_context.Orders.Local.Any(o => o.Id == order.Id))
        {
            _context.Orders.Attach(order);
        }

        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await OrderExists(order.Id))
            {
                throw new ArgumentException($"Order with ID '{order.Id}' does not exist.");
            }
            else
            {
                throw;
            }
        }
    }

    private async Task<bool> OrderExists(Guid orderId)
    {
        return await _context.Orders.AnyAsync(o => o.Id == orderId);
    }
}