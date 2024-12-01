using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

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
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders([FromQuery] string? status, [FromQuery] int? limit, [FromQuery] int? skip)
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
        public async Task<ActionResult> AddOrder([FromQuery] Guid businessId, [FromQuery] string status, [FromQuery] string currency)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdOrder = await _orderService.AddOrderAsync(businessId, status, currency);
                return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrder.Id }, null);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        // GET: api/Order/{orderId}
        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return NotFound();

            return Ok(order);
        }

        // DELETE: api/Order/{orderId}
        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return NotFound();

            await _orderService.DeleteOrderAsync(orderId);
            return NoContent();
        }

        // GET: api/Order/{orderId}/Transaction
        [HttpGet("{orderId}/Transaction")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAllTransactionsOfOrder(Guid orderId)
        {
            IEnumerable<Transaction> transactions;
            try
            {
                transactions = await _orderService.GetAllTransactionsOfOrderAsync(orderId);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }

            if(transactions == null)
                return NotFound();

            return Ok(transactions);
        }

        // POST: api/Order/{orderId}/Transaction
        [HttpPost("{orderId}/Transaction")]
        public async Task<ActionResult<IEnumerable<Transaction>>> ProcessAllTransactionsOfOrder(Guid orderId, [FromQuery] IEnumerable<Guid> itemIds, [FromQuery] decimal paidByCash, [FromQuery] decimal paidByGiftcard, [FromBody] Guid giftcardId)
        {
            // TODO Implement paying logic

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.ProcessAllTransactionsOfOrderAsync(orderId);

            throw new NotImplementedException();
        }

        // GET: api/Order/{orderId}/Transaction/{transactionId}
        [HttpGet("{orderId}/Transaction/{transactionId}")]
        public async Task<ActionResult<Order>> GetTransactionOfOrder(Guid orderId, Guid transactionId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return BadRequest();

            var transaction = await _orderService.GetTransactionByIdAsync(transactionId);
            if(transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        // POST: api/Order/{orderId}/Transaction/{transactionId}
        [HttpPost("{orderId}/Transaction/{transactionId}/refund")]
        public async Task<ActionResult<Order>> RefundTransaction(Guid orderId, Guid transactionId, [FromQuery] string refundMethod, [FromQuery] decimal amount)
        {
            // TODO Implement refund logic

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return NotFound();

            var transaction = await _orderService.GetTransactionByIdAsync(transactionId);
            if(transaction == null)
                return NotFound();

            await _orderService.RefundTransactionAsync(transaction);

            throw new NotImplementedException();
        }

        // GET: api/Order/{orderId}/Item
        [HttpGet("{orderId}/Item")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetAllItemsOfOrder(Guid orderId)
        {
            IEnumerable<OrderItem> items;
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
        public async Task<ActionResult> AddOrderItemToOrder(Guid orderId, [FromBody] OrderItem orderItem)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.GetOrderByIdAsync(orderId);
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

        // PUT: api/Order/{orderId}/Item
        [HttpPut("{orderId}/Item")]
        public async Task<ActionResult> UpdateOrderItem(Guid orderId, [FromBody] OrderItem orderItem)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return NotFound();

            try
            {
                await _orderService.UpdateOrderItemAsync(orderItem);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
            
            return Ok();
        }
    }
}
