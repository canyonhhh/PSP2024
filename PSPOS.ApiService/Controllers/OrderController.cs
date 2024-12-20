using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Schemas;
using Serilog;

namespace PSPOS.ApiService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: /orders
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderSchema>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IEnumerable<OrderSchema>>> GetAllOrders([FromQuery] string? status, [FromQuery] int? limit, [FromQuery] int? skip)
    {
        Log.Information("Fetching all orders with status: {Status}, limit: {Limit}, skip: {Skip}", status, limit, skip);
        try
        {
            var orders = await _orderService.GetAllOrdersAsync(status, limit, skip);
            return Ok(orders);
        }
        catch (ArgumentException ex)
        {
            Log.Error("Error fetching orders: {Message}", ex.Message);
            return BadRequest(new { ex.Message });
        }
    }

    // POST: /orders
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AddOrder([FromBody] OrderDTO orderDTO)
    {
        Log.Information("Adding new order for businessId: {BusinessId}", orderDTO.businessId);
        if (!ModelState.IsValid)
        {
            Log.Warning("Invalid model state for order: {ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            var createdOrder = await _orderService.AddOrderAsync(orderDTO.businessId, orderDTO.status, orderDTO.currency, orderDTO.createdBy);
            Log.Information("Order created successfully with ID: {OrderId}", createdOrder.Id);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrder.Id }, null);
        }
        catch (ArgumentException ex)
        {
            Log.Error("Error adding order: {Message}", ex.Message);
            return BadRequest(new { ex.Message });
        }
    }

    // GET: /orders/{orderId}
    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(OrderSchema), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderSchema>> GetOrderById(Guid orderId)
    {
        Log.Information("Fetching order with ID: {OrderId}", orderId);
        var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
        if (order == null)
        {
            Log.Warning("Order with ID: {OrderId} not found", orderId);
            return NotFound();
        }

        return Ok(order);
    }

    // DELETE: /orders/{orderId}
    [HttpDelete("{orderId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteOrder(Guid orderId)
    {
        Log.Information("Deleting order with ID: {OrderId}", orderId);
        var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
        if (order == null)
        {
            Log.Warning("Order with ID: {OrderId} not found", orderId);
            return NotFound();
        }

        await _orderService.DeleteOrderAsync(orderId);
        Log.Information("Order with ID: {OrderId} deleted successfully", orderId);
        return NoContent();
    }

    // GET: /orders/{orderId}/transactions
    [HttpGet("{orderId}/transactions")]
    [ProducesResponseType(typeof(IEnumerable<TransactionSchema>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<TransactionSchema>>> GetAllTransactionsOfOrder(Guid orderId)
    {
        Log.Information("Fetching all transactions for order ID: {OrderId}", orderId);
        IEnumerable<TransactionSchema> transactions;
        try
        {
            transactions = await _orderService.GetAllTransactionsOfOrderAsync(orderId);
        }
        catch (ArgumentException ex)
        {
            Log.Error("Error fetching transactions: {Message}", ex.Message);
            return BadRequest(new { ex.Message });
        }

        if (!transactions.Any())
        {
            Log.Warning("No transactions found for order ID: {OrderId}", orderId);
            return NotFound();
        }

        return Ok(transactions);
    }

    // POST: /orders/{orderId}/transactions
    [HttpPost("{orderId}/transactions")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> ProcessTransactionForOrder(Guid orderId, [FromBody] TransactionDTO transactionDTO)
    {
        Log.Information("Processing transaction for order ID: {OrderId}", orderId);
        if (!ModelState.IsValid)
        {
            Log.Warning("Invalid model state for transaction: {ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            await _orderService.ProcessTransactionForOrderAsync(orderId, transactionDTO);
            Log.Information("Transaction processed successfully for order ID: {OrderId}", orderId);
        }
        catch (ArgumentException ex)
        {
            Log.Error("Error processing transaction: {Message}", ex.Message);
            return BadRequest(new { ex.Message });
        }

        return Ok();
    }

    // GET: /orders/{orderId}/transactions/{transactionId}
    [HttpGet("{orderId}/transactions/{transactionId}")]
    [ProducesResponseType(typeof(TransactionSchema), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TransactionSchema>> GetTransactionOfOrder(Guid orderId, Guid transactionId)
    {
        Log.Information("Fetching transaction with ID: {TransactionId} for order ID: {OrderId}", transactionId, orderId);
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            Log.Warning("Order with ID: {OrderId} not found", orderId);
            return NotFound();
        }

        var transaction = await _orderService.GetTransactionSchemaByIdAsync(order, transactionId);
        if (transaction == null)
        {
            Log.Warning("Transaction with ID: {TransactionId} not found for order ID: {OrderId}", transactionId, orderId);
            return NotFound();
        }

        transaction.status = order.Status.ToString();

        return Ok(transaction);
    }

    // POST: /orders/{orderId}/transactions/{transactionId}
    [HttpPost("{orderId}/transactions/{transactionId}/refund")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> RefundTransaction(Guid orderId, Guid transactionId, [FromBody] RefundDTO refundDTO)
    {
        Log.Information("Refunding transaction with ID: {TransactionId} for order ID: {OrderId}", transactionId, orderId);
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            Log.Warning("Order with ID: {OrderId} not found", orderId);
            return NotFound();
        }

        var transactionSchema = await _orderService.GetTransactionSchemaByIdAsync(order, transactionId);
        if (transactionSchema == null)
        {
            Log.Warning("Transaction with ID: {TransactionId} not found for order ID: {OrderId}", transactionId, orderId);
            return NotFound();
        }

        try
        {
            await _orderService.RefundTransactionAsync(order, transactionSchema, refundDTO);
            Log.Information("Transaction with ID: {TransactionId} refunded successfully for order ID: {OrderId}", transactionId, orderId);
        }
        catch (ArgumentException ex)
        {
            Log.Error("Error refunding transaction: {Message}", ex.Message);
            return BadRequest(new { ex.Message });
        }

        return Ok();
    }

    // GET: /orders/{orderId}/items
    [HttpGet("{orderId}/items")]
    [ProducesResponseType(typeof(IEnumerable<OrderItemSchema>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<OrderItemSchema>>> GetAllItemsOfOrder(Guid orderId)
    {
        Log.Information("Fetching all items for order ID: {OrderId}", orderId);
        IEnumerable<OrderItemSchema> items;
        try
        {
            items = await _orderService.GetAllItemsOfOrderAsync(orderId);
        }
        catch (ArgumentException ex)
        {
            Log.Error("Error fetching items: {Message}", ex.Message);
            return BadRequest(new { ex.Message });
        }

        if (!items.Any())
        {
            Log.Warning("No items found for order ID: {OrderId}", orderId);
            return NotFound();
        }

        return Ok(items);
    }

    // POST: /orders/{orderId}/items
    [HttpPost("{orderId}/items")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AddOrderItemToOrder(Guid orderId, [FromBody] OrderItemDTO orderItem)
    {
        Log.Information("Adding item to order ID: {OrderId}", orderId);
        if (!ModelState.IsValid)
        {
            Log.Warning("Invalid model state for order item: {ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            await _orderService.AddOrderItemToOrderAsync(orderId, orderItem);
            Log.Information("Item added successfully to order ID: {OrderId}", orderId);
        }
        catch (ArgumentException ex)
        {
            Log.Error("Error adding item to order: {Message}", ex.Message);
            return BadRequest(new { ex.Message });
        }

        return Ok();
    }

    // PUT: /orders/{orderId}/items/{orderItemId}
    [HttpPut("{orderId}/items/{orderItemId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> UpdateOrderItem(Guid orderId, Guid orderItemId, [FromBody] OrderItemDTO orderItem)
    {
        Log.Information("Updating item with ID: {OrderItemId} for order ID: {OrderId}", orderItemId, orderId);
        if (!ModelState.IsValid)
        {
            Log.Warning("Invalid model state for order item: {ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
        if (order == null)
        {
            Log.Warning("Order with ID: {OrderId} not found", orderId);
            return NotFound();
        }

        try
        {
            await _orderService.UpdateOrderItemAsync(orderItemId, orderItem);
            Log.Information("Item with ID: {OrderItemId} updated successfully for order ID: {OrderId}", orderItemId, orderId);
        }
        catch (ArgumentException ex)
        {
            Log.Error("Error updating item: {Message}", ex.Message);
            return BadRequest(new { ex.Message });
        }

        return Ok();
    }

    [HttpPut("{orderId}")]
    [ProducesResponseType(typeof(OrderSchema), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderSchema>> UpdateOrder(Guid orderId, [FromBody] OrderDTO orderDTO)
    {
        Log.Information("Updating order with ID: {OrderId}", orderId);
        if (!ModelState.IsValid)
        {
            Log.Warning("Invalid model state for order: {ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(orderId, orderDTO);
            if (updatedOrder == null)
            {
                Log.Warning("Order with ID: {OrderId} not found", orderId);
                return NotFound(new { Message = $"Order with ID '{orderId}' not found." });
            }

            Log.Information("Order with ID: {OrderId} updated successfully", orderId);
            return Ok(updatedOrder);
        }
        catch (ArgumentException ex)
        {
            Log.Error("Error updating order: {Message}", ex.Message);
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            Log.Error("An unexpected error occurred while updating order with ID: {OrderId}. Error: {Message}", orderId, ex.Message);
            return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        }
    }
}