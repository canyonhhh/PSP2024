using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.Schemas;

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

    public async Task<OrderSchema?> GetOrderSchemaByIdAsync(Guid id)
    {
        Order? order = await _orderRepository.GetOrderByIdAsync(id);
        if(order == null)
            return null;

        return new()
        {
            Id = order.Id,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            CreatedBy = order.CreatedBy,
            UpdatedBy = order.UpdatedBy,
            businessId = order.BusinessId,
            status = order.Status.ToString(),
            currency = order.OrderCurrency.ToString()
        };
    }

    public async Task<IEnumerable<OrderSchema>> GetAllOrdersAsync(string? status, int? limit, int? skip)
    {
        IEnumerable<Order> items = await _orderRepository.GetAllOrdersAsync(status, limit, skip);

        return items.Select(item => new OrderSchema
        {
            Id = item.Id,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            CreatedBy = item.CreatedBy,
            UpdatedBy = item.UpdatedBy,
            businessId = item.BusinessId,
            status = item.Status.ToString(),
            currency = item.OrderCurrency.ToString()
        });
    }

    public async Task<Order> AddOrderAsync(Guid businessId, string? status, string? currency)
    {
        return await _orderRepository.AddOrderAsync(businessId, status, currency);
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        await _orderRepository.DeleteOrderAsync(id);
    }

    public async Task<IEnumerable<TransactionSchema>> GetAllTransactionsOfOrderAsync(Guid orderId)
    {
        Order? order = await _orderRepository.GetOrderByIdAsync(orderId);
        if(order == null)
            return [];

        IEnumerable<Transaction> transactions = await _orderRepository.GetAllTransactionsOfOrderAsync(orderId);

        var transactionSchemas = await Task.WhenAll(transactions.Select(transaction => GetTransactionSchemaByIdAsync(transaction.Id)));

        return (IEnumerable<TransactionSchema>)transactionSchemas.Where(ts => ts != null);
    }

    public async Task ProcessAllTransactionsOfOrderAsync(Guid id)
    {
        await _orderRepository.ProcessAllTransactionsOfOrderAsync(id);
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        return await _orderRepository.GetTransactionByIdAsync(id);
    }

    public async Task<TransactionSchema?> GetTransactionSchemaByIdAsync(Guid id)
    {
        var transaction = await _orderRepository.GetTransactionByIdAsync(id);
        if(transaction == null)
            return null;

        var payments = await _orderRepository.GetAllPaymentsOfTransacionAsync(id);
        if(!payments.Any())
            return null;

        TransactionSchema transactionSchema = new()
        {
            id = transaction.Id
        };

        foreach(var payment in payments)
        {
            if(payment.Method == PaymentMethod.Cash)
                transactionSchema.paidByCash += payment.Amount;
            else if(payment.Method == PaymentMethod.Giftcard)
                transactionSchema.paidByGiftcard += payment.Amount;
            // TODO Stripe platform external, I think
        }

        return transactionSchema;
    }

    public Task RefundTransactionAsync(Order order, Transaction transaction, RefundDTO refundDTO)
    {
        // TODO Implement
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<OrderItemSchema>> GetAllItemsOfOrderAsync(Guid id)
    {
        IEnumerable<OrderItem> items = await _orderRepository.GetAllItemsOfOrderAsync(id);

        return items.Select(item => new OrderItemSchema
        {
            Id = item.Id,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            CreatedBy = item.CreatedBy,
            UpdatedBy = item.UpdatedBy,
            type = item.Type.ToString(),
            price = item.Price,
            quantity = item.Quantity,
            orderId = item.OrderId,
            serviceId = item.ServiceId,
            productId = item.ProductId,
            transactionId = item.TransactionId
        });
    }

    public async Task AddOrderItemToOrderAsync(OrderItemDTO orderItemDTO)
    {
        if(!Enum.TryParse(orderItemDTO.type, true, out OrderItemType orderItemType))
            throw new ArgumentException($"Order item type '{orderItemType}' does not exist."); // Catch in controller

        OrderItem orderItem = new
        (
            orderItemType,
            orderItemDTO.price,
            orderItemDTO.quantity,
            orderItemDTO.orderId,
            orderItemDTO.serviceId,
            orderItemDTO.productId,
            orderItemDTO.transactionId
        );

        await _orderRepository.AddOrderItemToOrderAsync(orderItem);
    }

    public async Task UpdateOrderItemAsync(Guid orderItemId, OrderItemDTO orderItemDTO)
    {
        if(!Enum.TryParse(orderItemDTO.type, true, out OrderItemType orderItemType))
            throw new ArgumentException($"Order item type '{orderItemType}' does not exist."); // Catch in controller

        OrderItem orderItem = new
        (
            orderItemType,
            orderItemDTO.price,
            orderItemDTO.quantity,
            orderItemDTO.orderId,
            orderItemDTO.serviceId,
            orderItemDTO.productId,
            orderItemDTO.transactionId
        )
        {
            Id = orderItemId
        };

        await _orderRepository.UpdateOrderItemAsync(orderItem);
    }
}
