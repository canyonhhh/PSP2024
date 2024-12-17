using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.Schemas;

namespace PSPOS.ApiService.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProdAndServRepository _prodAndServRepository;

    public OrderService(IOrderRepository orderRepository, IProdAndServRepository prodAndServRepository)
    {
        _orderRepository = orderRepository;
        _prodAndServRepository = prodAndServRepository;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _orderRepository.GetOrderByIdAsync(id);
    }
    public async Task<Order?> UpdateOrderAsync(Guid orderId, OrderDTO orderDTO)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
            return null;

        if (orderDTO.tip.HasValue)
            order.Tip = orderDTO.tip.Value;

        if (!string.IsNullOrEmpty(orderDTO.status))
            order.Status = Enum.Parse<OrderStatus>(orderDTO.status, true);

        await _orderRepository.UpdateOrder(order);

        return order;
    }


    public async Task<OrderSchema?> GetOrderSchemaByIdAsync(Guid id)
    {
        Order? order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
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
            currency = item.OrderCurrency.ToString(),
            tip = item.Tip
        });
    }

    public async Task<Order> AddOrderAsync(Guid businessId, string? status, string? currency, Guid createdBy)
    {
        return await _orderRepository.AddOrderAsync(businessId, status, currency, createdBy);
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        await DuplicateAllItemsOfOrderToInventory(id);
        await _orderRepository.DeleteOrderAsync(id);
    }

    public async Task<IEnumerable<TransactionSchema>> GetAllTransactionsOfOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId) ?? throw new ArgumentException($"Order '{orderId}' does not exist.");

        IEnumerable<Transaction> transactions = await _orderRepository.GetAllTransactionsOfOrderAsync(orderId);
        List<TransactionSchema> transactionSchemas = [];

        foreach (Transaction transaction in transactions)
        {
            TransactionSchema? transactionSchema = await GetTransactionSchemaByIdAsync(order, transaction.Id);
            if (transactionSchema != null)
                transactionSchemas.Add(transactionSchema);
        }

        return transactionSchemas;
    }

    public async Task ProcessTransactionForOrderAsync(Guid orderId, TransactionDTO transactionDTO)
    {
        Order? order = await _orderRepository.GetOrderByIdAsync(orderId) ?? throw new ArgumentException($"Order '{orderId}' does not exist.");
        if (order.Status != OrderStatus.Open)
            throw new ArgumentException($"Order '{orderId}' is not open.");

        if (!(transactionDTO.itemIds?.Count() > 0))
            throw new ArgumentException($"Item IDs were not provided in request.");

        IEnumerable<OrderItem> orderItems = await _orderRepository.GetAllItemsOfOrderAsyncO(orderId);
        if (!orderItems.Any())
            throw new ArgumentException($"Order '{orderId}' doesn't contain any items.");

        IEnumerable<OrderItem> orderItemsToLinkToTransaction = orderItems.Where(oi => transactionDTO.itemIds.Contains(oi.Id)).Distinct();
        if (orderItemsToLinkToTransaction.Count() != transactionDTO.itemIds.Count())
            throw new ArgumentException($"There are item IDs in request that the order doesn't contain or there are duplicate item IDs.");

        // Check if already paid for
        foreach (OrderItem orderItem in orderItemsToLinkToTransaction)
            if (orderItem.TransactionId != Guid.Empty)
                throw new ArgumentException($"Item '{orderItem.Id}' is already paid for.");


        // Check if enough money is paid for all the items
        // TODO Use Stripe too
        //decimal price = 0;
        //foreach (OrderItem orderItem in orderItemsToLinkToTransaction)
        //    price += orderItem.Price;
        //if (price < transactionDTO.paidByCash + transactionDTO.paidByGiftcard)
        //    throw new ArgumentException($"Paid too much, expected {price}{order.OrderCurrency}.");
        //else if (price > transactionDTO.paidByCash + transactionDTO.paidByGiftcard)
        //    throw new ArgumentException($"Paid too little, expected {price}{order.OrderCurrency}.");

        // Add transaction data to database
        Transaction transaction = new(TransactionType.Purchase);

        if (transactionDTO.paidByGiftcard > 0 && transactionDTO.giftcardCode != null)
        {
            // Retrieve the gift card by its code
            Giftcard? giftcard = await _orderRepository.GetGiftCardByCode(transactionDTO.giftcardCode);

            if (giftcard == null)
            {
                throw new InvalidOperationException("Gift card not found.");
            }

            // Deduct the payment amount from the gift card balance
            if (giftcard.Amount < transactionDTO.paidByGiftcard)
            {
                throw new InvalidOperationException("Insufficient balance on the gift card.");
            }
          
            giftcard.Amount -= transactionDTO.paidByGiftcard;

            // Create a new payment for the gift card
            Payment giftcardPayment = new(
                PaymentMethod.Giftcard,
                transactionDTO.paidByGiftcard,
                order.OrderCurrency,
                string.Empty,
                transaction.Id,
                giftcard.Id
            );

            // Add the payment and update the gift card amount
            await _orderRepository.AddPaymentAsync(giftcardPayment);
            await _orderRepository.UpdateGiftCardAmountAsync(giftcard);
        }


        if (transactionDTO.paidByCash > 0)
        {
            Payment cashPayment = new(PaymentMethod.Cash, transactionDTO.paidByCash, order.OrderCurrency, String.Empty, transaction.Id, Guid.Empty);
            await _orderRepository.AddPaymentAsync(cashPayment);
        }

        if (transactionDTO.paidByBankcard > 0 && transactionDTO.externalTransactionId != null)
        {
            Payment bankcardPayment = new(PaymentMethod.Bankcard, transactionDTO.paidByBankcard, order.OrderCurrency, transactionDTO.externalTransactionId, transaction.Id, Guid.Empty);
            await _orderRepository.AddPaymentAsync(bankcardPayment);
        }

        // Order items get a transaction ID to tell if they're paid for
        foreach (OrderItem orderItem in orderItemsToLinkToTransaction)
        {
            orderItem.TransactionId = transaction.Id;
            await _orderRepository.UpdateOrderItemAsync(orderItem);
        }

        await _orderRepository.AddTransactionAsync(transaction);

        // If all items are paid for, close the order
        if (!orderItems.Where(oi => oi.TransactionId == Guid.Empty).Any())
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
        if (transaction == null)
            return null;

        var payments = await _orderRepository.GetAllPaymentsOfTransactionAsync(transactionId);
        if (!payments.Any())
            return null;

        TransactionSchema transactionSchema = new()
        {
            id = transaction.Id,
            status = order.Status.ToString()
        };

        foreach (var payment in payments)
        {
            if (payment.Method == PaymentMethod.Cash)
                transactionSchema.paidByCash += payment.Amount;
            else if (payment.Method == PaymentMethod.Giftcard)
                transactionSchema.paidByGiftcard += payment.Amount;
            // TODO Stripe payment amount
        }

        return transactionSchema;
    }

    public async Task RefundTransactionAsync(Order order, TransactionSchema oldTransactionSchema, RefundDTO refundDTO)
    {
        if (!Enum.TryParse(refundDTO.refundMethod, true, out PaymentMethod refundMethod))
            throw new ArgumentException($"Refund method '{refundDTO.refundMethod}' does not exist.");

        if (refundMethod == PaymentMethod.Giftcard)
            throw new ArgumentException($"Refunding to giftcard is not possible.");

        Transaction refundTransaction = new(TransactionType.Refund);

        switch (refundMethod)
        {
            case PaymentMethod.Cash:
                Payment cashPayment = new(PaymentMethod.Cash, refundDTO.amount, order.OrderCurrency, String.Empty, refundTransaction.Id, Guid.Empty);
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
        // Retrieve data from the repository
        IEnumerable<OrderItemSchema> items = await _orderRepository.GetAllItemsOfOrderAsync(id);

        // Map and ensure null handling for appliedDiscounts and appliedTaxes
        return items.Select(item => new OrderItemSchema
        {
            Id = item.Id,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            CreatedBy = item.CreatedBy,
            UpdatedBy = item.UpdatedBy,
            type = item.type?.ToString() ?? "Unknown",
            price = item.price,
            quantity = item.quantity,
            orderId = item.orderId,
            serviceId = item.serviceId,
            productId = item.productId,
            transactionId = item.transactionId,

            // Ensure appliedDiscounts and appliedTaxes are never null
            appliedDiscounts = item.appliedDiscounts?.ToList() ?? new List<AppliedDiscountSchema>(),
            appliedTaxes = item.appliedTaxes?.ToList() ?? new List<AppliedTaxSchema>()
        });
    }


    public async Task AddOrderItemToOrderAsync(Guid orderId, OrderItemDTO orderItemDTO)
    {
        if (orderItemDTO.quantity < 1)
            throw new ArgumentException($"Order item quantity must be more than 0.");

        if (!Enum.TryParse(orderItemDTO.type, true, out OrderItemType orderItemType))
            throw new ArgumentException($"Order item type '{orderItemDTO.type}' does not exist.");

        Order? order = await _orderRepository.GetOrderByIdAsync(orderItemDTO.orderId)
            ?? throw new ArgumentException($"Order '{orderItemDTO.orderId}' does not exist.");

        if (order.Id != orderId)
            throw new ArgumentException($"Order ID '{orderId}' doesn't match with order item's order ID {orderItemDTO.orderId}.");

        if (order.Status != OrderStatus.Open)
            throw new ArgumentException($"Order '{orderItemDTO.orderId}' is not open.");

        if (orderItemDTO.transactionId != Guid.Empty && (await _orderRepository.GetTransactionByIdAsync(orderItemDTO.transactionId)) == null)
            throw new ArgumentException($"Transaction '{orderItemDTO.transactionId}' does not exist.");

        // Product quantity
        if (orderItemType == OrderItemType.Product)
        {
            // Checking
            var productStock = await _prodAndServRepository.GetProductStockAsync(orderItemDTO.productId)
                ?? throw new ArgumentException($"Product '{orderItemDTO.productId}' does not exist.");

            if (productStock.Quantity < orderItemDTO.quantity)
                throw new ArgumentException($"Insufficient quantity of product '{orderItemDTO.productId}' in stock.");

            // Updating
            productStock.Quantity -= orderItemDTO.quantity;
            await _prodAndServRepository.UpdateProductStockAsync(productStock);
        }

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
        if (orderItemDTO.quantity < 1)
            throw new ArgumentException($"Order item quantity must be more than 0.");

        if (!Enum.TryParse(orderItemDTO.type, true, out OrderItemType orderItemType))
            throw new ArgumentException($"Order item type '{orderItemDTO.type}' does not exist.");

        var existingOrderItem = await _orderRepository.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException($"Order item '{orderItemId}' does not exist.");

        if (existingOrderItem.OrderId != orderItemDTO.orderId)
            throw new ArgumentException($"Order ID '{orderItemDTO.orderId}' does not match the order item's existing Order ID '{existingOrderItem.OrderId}'.");

        Order? order = await _orderRepository.GetOrderByIdAsync(existingOrderItem.OrderId)
            ?? throw new ArgumentException($"Order '{existingOrderItem.OrderId}' does not exist.");

        if (order.Status != OrderStatus.Open)
            throw new ArgumentException($"Order '{existingOrderItem.OrderId}' is not open.");

        // Product quantity
        if (orderItemType == OrderItemType.Product)
        {
            // Checking
            var productStock = await _prodAndServRepository.GetProductStockAsync(orderItemDTO.productId)
                ?? throw new ArgumentException($"Product '{orderItemDTO.productId}' does not exist.");

            int quantityDifferenceInNewOrderItem = orderItemDTO.quantity - existingOrderItem.Quantity;
            if (productStock.Quantity < quantityDifferenceInNewOrderItem)
                throw new ArgumentException($"Insufficient quantity of product '{orderItemDTO.productId}' in stock.");

            // Updating
            productStock.Quantity -= quantityDifferenceInNewOrderItem;
            await _prodAndServRepository.UpdateProductStockAsync(productStock);
        }

        // Update the order item
        existingOrderItem.Type = orderItemType;
        existingOrderItem.Price = orderItemDTO.price;
        existingOrderItem.Quantity = orderItemDTO.quantity;
        existingOrderItem.ServiceId = orderItemDTO.serviceId;
        existingOrderItem.ProductId = orderItemDTO.productId;
        existingOrderItem.TransactionId = orderItemDTO.transactionId;

        await _orderRepository.UpdateOrderItemAsync(existingOrderItem);
    }

    public async Task DuplicateAllItemsOfOrderToInventory(Guid orderId)
    {
        IEnumerable<OrderItem> orderItems = await _orderRepository.GetAllItemsOfOrderAsyncO(orderId);

        foreach (OrderItem item in orderItems)
        {
            if (item.Type == OrderItemType.Product)
            {
                var productStock = await _prodAndServRepository.GetProductStockAsync(item.ProductId);

                if (productStock != null)
                {
                    productStock.Quantity += item.Quantity;
                    await _prodAndServRepository.UpdateProductStockAsync(productStock);
                }
            }
        }
    }

}