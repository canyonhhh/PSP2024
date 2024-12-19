using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Schemas;

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
        try
        {
            var orders = await _orderService.GetAllOrdersAsync(status, limit, skip);
            return Ok(orders);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    // POST: /orders
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AddOrder([FromBody] OrderDTO orderDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdOrder = await _orderService.AddOrderAsync(orderDTO.businessId, orderDTO.status, orderDTO.currency, orderDTO.createdBy);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrder.Id }, null);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    // GET: /orders/{orderId}
    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(OrderSchema), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderSchema>> GetOrderById(Guid orderId)
    {
        var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    // DELETE: /orders/{orderId}
    [HttpDelete("{orderId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteOrder(Guid orderId)
    {
        var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
        if (order == null)
            return NotFound();

        await _orderService.DeleteOrderAsync(orderId);
        return NoContent();
    }

    // GET: /orders/{orderId}/transactions
    [HttpGet("{orderId}/transactions")]
    [ProducesResponseType(typeof(IEnumerable<TransactionSchema>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<TransactionSchema>>> GetAllTransactionsOfOrder(Guid orderId)
    {
        IEnumerable<TransactionSchema> transactions;
        try
        {
            transactions = await _orderService.GetAllTransactionsOfOrderAsync(orderId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }

        if (!transactions.Any())
            return NotFound();

        return Ok(transactions);
    }

    // POST: /orders/{orderId}/transactions
    [HttpPost("{orderId}/transactions")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> ProcessTransactionForOrder(Guid orderId, [FromBody] TransactionDTO transactionDTO)
    {

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _orderService.ProcessTransactionForOrderAsync(orderId, transactionDTO);
        }
        catch (ArgumentException ex)
        {
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
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
            return NotFound();

        var transaction = await _orderService.GetTransactionSchemaByIdAsync(order, transactionId);
        if (transaction == null)
            return NotFound();

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
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
            return NotFound();

        var transactionSchema = await _orderService.GetTransactionSchemaByIdAsync(order, transactionId);
        if (transactionSchema == null)
            return NotFound();

        try
        {
            await _orderService.RefundTransactionAsync(order, transactionSchema, refundDTO);
        }
        catch (ArgumentException ex)
        {
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
        IEnumerable<OrderItemSchema> items;
        try
        {
            items = await _orderService.GetAllItemsOfOrderAsync(orderId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }

        if (!items.Any())
            return NotFound();

        return Ok(items);
    }

    // POST: /orders/{orderId}/items
    [HttpPost("{orderId}/items")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AddOrderItemToOrder(Guid orderId, [FromBody] OrderItemDTO orderItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _orderService.AddOrderItemToOrderAsync(orderId, orderItem);
        }
        catch (ArgumentException ex)
        {
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
        if (order == null)
            return NotFound();

        try
        {
            await _orderService.UpdateOrderItemAsync(orderItemId, orderItem);
        }
        catch (ArgumentException ex)
        {
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(orderId, orderDTO);
            if (updatedOrder == null)
                return NotFound(new { Message = $"Order with ID '{orderId}' not found." });

            return Ok(updatedOrder);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        }
    }
}