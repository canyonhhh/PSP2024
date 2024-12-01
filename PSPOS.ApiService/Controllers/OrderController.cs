using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Schemas;

namespace PSPOS.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderSchema>>> GetAllOrders([FromQuery] string? status, [FromQuery] int? limit, [FromQuery] int? skip)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(status, limit, skip);
                return Ok(orders);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult> AddOrder([FromBody] OrderDTO orderDTO)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdOrder = await _orderService.AddOrderAsync(orderDTO.businessId, orderDTO.status, orderDTO.currency);
                return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrder.Id }, null);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        // GET: api/Order/{orderId}
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderSchema>> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
            if(order == null)
                return NotFound();

            return Ok(order);
        }

        // DELETE: api/Order/{orderId}
        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(Guid orderId)
        {
            var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
            if(order == null)
                return NotFound();

            await _orderService.DeleteOrderAsync(orderId);
            return NoContent();
        }

        // GET: api/Order/{orderId}/Transaction
        [HttpGet("{orderId}/Transaction")]
        public async Task<ActionResult<IEnumerable<TransactionSchema>>> GetAllTransactionsOfOrder(Guid orderId)
        {
            IEnumerable<TransactionSchema> transactions;
            try
            {
                transactions = await _orderService.GetAllTransactionsOfOrderAsync(orderId);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }

            return Ok(transactions);
        }

        // POST: api/Order/{orderId}/Transaction
        [HttpPost("{orderId}/Transaction")]
        public async Task<ActionResult<IEnumerable<Transaction>>> ProcessAllTransactionsOfOrder(Guid orderId, [FromBody] TransactionDTO transactionDTO)
        {
            // TODO Implement paying logic

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.ProcessAllTransactionsOfOrderAsync(orderId);

            throw new NotImplementedException();
        }

        // GET: api/Order/{orderId}/Transaction/{transactionId}
        [HttpGet("{orderId}/Transaction/{transactionId}")]
        public async Task<ActionResult<TransactionSchema>> GetTransactionOfOrder(Guid orderId, Guid transactionId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return BadRequest();

            var transaction = await _orderService.GetTransactionSchemaByIdAsync(transactionId);
            if(transaction == null)
                return NotFound();

            transaction.status = order.Status.ToString();

            return Ok(transaction);
        }

        // POST: api/Order/{orderId}/Transaction/{transactionId}
        [HttpPost("{orderId}/Transaction/{transactionId}/refund")]
        public async Task<ActionResult<Order>> RefundTransaction(Guid orderId, Guid transactionId, [FromBody] RefundDTO refundDTO)
        {
            // TODO Implement refund logic

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return NotFound();

            var transaction = await _orderService.GetTransactionByIdAsync(transactionId);
            if(transaction == null)
                return NotFound();

            await _orderService.RefundTransactionAsync(order, transaction, refundDTO);

            throw new NotImplementedException();
        }

        // GET: api/Order/{orderId}/Item
        [HttpGet("{orderId}/Item")]
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

            if(items == null)
                return NotFound();

            return Ok(items);
        }

        // POST: api/Order/{orderId}/Item
        [HttpPost("{orderId}/Item")]
        public async Task<ActionResult> AddOrderItemToOrder(Guid orderId, [FromBody] OrderItemDTO orderItem)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
            if(order == null)
                return NotFound();

            try
            {
                await _orderService.AddOrderItemToOrderAsync(orderItem);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }

            return Ok();
        }

        // PUT: api/Order/{orderId}/Item/{orderItemId}
        [HttpPut("{orderId}/Item/{orderItemId}")]
        public async Task<ActionResult> UpdateOrderItem(Guid orderId, Guid orderItemId, [FromBody] OrderItemDTO orderItem)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.GetOrderSchemaByIdAsync(orderId);
            if(order == null)
                return NotFound();

            try
            {
                await _orderService.UpdateOrderItemAsync(orderItemId, orderItem);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
            
            return Ok();
        }
    }
}
