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
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult> AddOrder([FromBody] Order order)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.AddOrderAsync(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
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
            var transactions = await _orderService.GetAllTransactionsOfOrderAsync(orderId);
            if(transactions == null)
                return NotFound();

            return Ok(transactions);
        }

        // POST: api/Order/{orderId}/Transaction
        [HttpPost("{orderId}/Transaction")]
        public async Task<ActionResult<IEnumerable<Transaction>>> ProcessAllTransactionsOfOrder(Guid orderId)
        {
            // TODO Implement

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.ProcessAllTransactionsOfOrderAsync(orderId);
            // TODO Create a way to get success/failure confirmation
            throw new NotImplementedException();
        }

        // GET: api/Order/{orderId}/Transaction/{transactionId}
        [HttpGet("{orderId}/Transaction/{transactionId}")]
        public async Task<ActionResult<Order>> GetTransactionOfOrder(Guid orderId, Guid transactionId)
        {
            // `orderId` for making sure the order exists, I guess
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return NotFound();

            var transaction = await _orderService.GetTransactionByIdAsync(transactionId);
            if(transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        // POST: api/Order/{orderId}/Transaction/{transactionId}
        [HttpPost("{orderId}/Transaction/{transactionId}/refund")]
        public async Task<ActionResult<Order>> RefundTransactionOfOrder(Guid orderId, Guid transactionId)
        {
            // TODO Implement

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return NotFound();

            await _orderService.RefundTransactionByIdAsync(transactionId);
            // TODO Create a way to get success/failure confirmation
            throw new NotImplementedException();
        }

        // GET: api/Order/{orderId}/Item
        [HttpGet("{orderId}/Item")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetAllItemsOfOrder(Guid orderId)
        {
            var items = await _orderService.GetAllItemsOfOrderAsync(orderId);
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

            await _orderService.AddOrderItemToOrderAsync(orderItem);
            // TODO Add check whether item was added and send it
            return Ok();
        }

        // PUT: api/Order/{orderId}/Item
        [HttpPut("{orderId}/Item")]
        public async Task<ActionResult> UpdateOrderItemOfOrder(Guid orderId, [FromBody] OrderItem orderItem)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if(order == null)
                return NotFound();

            await _orderService.UpdateOrderItemOfOrderAsync(orderItem);
            // TODO Add check whether item was updated and send it
            return Ok();
        }
    }
}
