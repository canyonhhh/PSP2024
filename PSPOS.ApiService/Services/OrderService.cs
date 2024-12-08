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
        var order = await _orderRepository.GetOrderByIdAsync(orderId) ?? throw new ArgumentException($"Order '{orderId}' does not exist.");
        
        IEnumerable<Transaction> transactions = await _orderRepository.GetAllTransactionsOfOrderAsync(orderId);
        List<TransactionSchema> transactionSchemas = [];

        foreach(Transaction transaction in transactions)
        {
            TransactionSchema? transactionSchema = await GetTransactionSchemaByIdAsync(order, transaction.Id);
            if(transactionSchema != null)
                transactionSchemas.Add(transactionSchema);
        }

        return transactionSchemas;
    }

    public async Task ProcessTransactionForOrderAsync(Guid orderId, TransactionDTO transactionDTO)
    {
        Order? order = await _orderRepository.GetOrderByIdAsync(orderId) ?? throw new ArgumentException($"Order '{orderId}' does not exist.");
        if(order.Status != OrderStatus.Open)
            throw new ArgumentException($"Order '{orderId}' is not open.");

        if(!(transactionDTO.itemIds?.Count() > 0))
            throw new ArgumentException($"Item IDs were not provided in request.");

        IEnumerable<OrderItem> orderItems = await _orderRepository.GetAllItemsOfOrderAsync(orderId);
        if(!orderItems.Any())
            throw new ArgumentException($"Order '{orderId}' doesn't contain any items.");

        IEnumerable<OrderItem> orderItemsToLinkToTransaction = orderItems.Where(oi => transactionDTO.itemIds.Contains(oi.Id)).Distinct();
        if(orderItemsToLinkToTransaction.Count() != transactionDTO.itemIds.Count())
            throw new ArgumentException($"There are item IDs in request that the order doesn't contain or there are duplicate item IDs.");

        // Check if already paid for
        foreach(OrderItem orderItem in orderItemsToLinkToTransaction)
            if(orderItem.TransactionId != Guid.Empty)
                throw new ArgumentException($"Item '{orderItem.Id}' is already paid for.");

        // Check for valid gift card
        if(transactionDTO.paidByGiftcard > 0)
        {
            if(transactionDTO.giftcardId == Guid.Empty)
                throw new ArgumentException($"Giftcard payment amount provided with no giftcard ID.");

            Giftcard? giftCard = await _orderRepository.GetGiftcardByIdAsync(transactionDTO.giftcardId) ?? throw new ArgumentException($"Giftcard '{transactionDTO.giftcardId}' doesn't exist.");

            if(giftCard.Amount < transactionDTO.paidByGiftcard)
                throw new ArgumentException($"Giftcard '{transactionDTO.giftcardId}' doesn't have enough money for the transaction.");
        }

        // Check if enough money is paid for all the items
        // TODO Use Stripe too
        decimal price = 0;
        foreach(OrderItem orderItem in orderItemsToLinkToTransaction)
            price += orderItem.Price;
        if(price < transactionDTO.paidByCash + transactionDTO.paidByGiftcard)
            throw new ArgumentException($"Paid too much, expected {price}{order.OrderCurrency}.");
        else if(price > transactionDTO.paidByCash + transactionDTO.paidByGiftcard)
            throw new ArgumentException($"Paid too little, expected {price}{order.OrderCurrency}.");

        // Add transaction data to database
        Transaction transaction = new(TransactionType.Purchase);

        if(transactionDTO.paidByGiftcard > 0)
        {
            Payment giftcardPayment = new(PaymentMethod.Giftcard, transactionDTO.paidByGiftcard, order.OrderCurrency, Guid.Empty, transaction.Id, transactionDTO.giftcardId);
            await _orderRepository.AddPaymentAsync(giftcardPayment);
        }

        if(transactionDTO.paidByCash > 0)
        {
            Payment cashPayment = new(PaymentMethod.Cash, transactionDTO.paidByCash, order.OrderCurrency, Guid.Empty, transaction.Id, Guid.Empty);
            await _orderRepository.AddPaymentAsync(cashPayment);
        }

        // TODO Stripe platform payment

        // Order items get a transaction ID to tell if they're paid for
        foreach(OrderItem orderItem in orderItemsToLinkToTransaction)
        {
            orderItem.TransactionId = transaction.Id;
            await _orderRepository.UpdateOrderItemAsync(orderItem);
        }

        await _orderRepository.AddTransactionAsync(transaction);

        // If all items are paid for, close the order
        if(!orderItems.Where(oi => oi.TransactionId == Guid.Empty).Any())
        {
            order.Status = OrderStatus.Closed;
            await _orderRepository.UpdateOrder(order);
        }
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        return await _orderRepository.GetTransactionByIdAsync(id);
    }

    public async Task<TransactionSchema?> GetTransactionSchemaByIdAsync(Order order, Guid transactionId)
    {
        var transaction = await _orderRepository.GetTransactionByIdAsync(transactionId);
        if(transaction == null)
            return null;

        var payments = await _orderRepository.GetAllPaymentsOfTransacionAsync(transactionId);
        if(!payments.Any())
            return null;

        TransactionSchema transactionSchema = new()
        {
            id = transaction.Id,
            status = order.Status.ToString()
        };

        foreach(var payment in payments)
        {
            if(payment.Method == PaymentMethod.Cash)
                transactionSchema.paidByCash += payment.Amount;
            else if(payment.Method == PaymentMethod.Giftcard)
                transactionSchema.paidByGiftcard += payment.Amount;
            // TODO Stripe payment amount
        }

        return transactionSchema;
    }

    public async Task RefundTransactionAsync(Order order, TransactionSchema oldTransactionSchema, RefundDTO refundDTO)
    {
        if(!Enum.TryParse(refundDTO.refundMethod, true, out PaymentMethod refundMethod))
            throw new ArgumentException($"Refund method '{refundDTO.refundMethod}' does not exist.");

        if(refundMethod == PaymentMethod.Giftcard)
            throw new ArgumentException($"Refunding to giftcard is not possible.");

        Transaction refundTransaction = new(TransactionType.Refund);

        switch(refundMethod)
        {
            case PaymentMethod.Cash:
                Payment cashPayment = new(PaymentMethod.Cash, refundDTO.amount, order.OrderCurrency, Guid.Empty, refundTransaction.Id, Guid.Empty);
                await _orderRepository.AddPaymentAsync(cashPayment);
                break;
                // TODO Case for Stripe platform refund
        }

        await _orderRepository.AddTransactionAsync(refundTransaction);

        // Mark order as refunded
        order.Status = OrderStatus.Refunded;
        await _orderRepository.UpdateOrder(order);
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

    // TODO Check item availability in inventory
    public async Task AddOrderItemToOrderAsync(Guid orderItemId, OrderItemDTO orderItemDTO)
    {
        if(orderItemDTO.quantity < 1)
            throw new ArgumentException($"Order item quantity must be more than 0.");

        if(!Enum.TryParse(orderItemDTO.type, true, out OrderItemType orderItemType))
            throw new ArgumentException($"Order item type '{orderItemDTO.type}' does not exist.");

        Order? order = await _orderRepository.GetOrderByIdAsync(orderItemDTO.orderId) ?? throw new ArgumentException($"Order '{orderItemDTO.orderId}' does not exist.");
        if(order.Id != orderItemId)
            throw new ArgumentException($"Order id '{orderItemId}' doesn't match with order item's order id {orderItemDTO.orderId}.");
        if(order.Status != OrderStatus.Open)
            throw new ArgumentException($"Order '{orderItemDTO.orderId}' is not open.");

        if(orderItemDTO.transactionId != Guid.Empty && (await _orderRepository.GetTransactionByIdAsync(orderItemDTO.transactionId)) == null)
            throw new ArgumentException($"Transaction '{orderItemDTO.transactionId}' does not exist.");

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

    // TODO Check item availability in inventory
    public async Task UpdateOrderItemAsync(Guid orderItemId, OrderItemDTO orderItemDTO)
    {
        if(orderItemDTO.quantity < 1)
            throw new ArgumentException($"Order item quantity must be more than 0.");

        if(!Enum.TryParse(orderItemDTO.type, true, out OrderItemType orderItemType))
            throw new ArgumentException($"Order item type '{orderItemDTO.type}' does not exist.");

        Order? order = await _orderRepository.GetOrderByIdAsync(orderItemDTO.orderId) ?? throw new ArgumentException($"Order '{orderItemDTO.orderId}' does not exist.");
        if(order.Id != orderItemId)
            throw new ArgumentException($"Order id '{orderItemId}' doesn't match with order item's order id {orderItemDTO.orderId}.");
        if(order.Status != OrderStatus.Open)
            throw new ArgumentException($"Order '{orderItemDTO.orderId}' is not open.");

        if(orderItemDTO.transactionId != Guid.Empty && (await _orderRepository.GetTransactionByIdAsync(orderItemDTO.transactionId)) == null)
            throw new ArgumentException($"Transaction '{orderItemDTO.transactionId}' does not exist.");

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
