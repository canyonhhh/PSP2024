using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using Serilog;

[ApiController]
[Route("api/discounts")]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountService _discountService;

    public DiscountsController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Discount>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<Discount>>> GetAllDiscounts(
        [FromQuery] string? from,
        [FromQuery] string? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { Message = "Page and pageSize must be positive integers." });

            if (from == null && !string.IsNullOrEmpty(from))
                return BadRequest(new { Message = "Invalid 'from' date format. Use ISO 8601 (UTC)." });

            if (to == null && !string.IsNullOrEmpty(to))
                return BadRequest(new { Message = "Invalid 'to' date format. Use ISO 8601 (UTC)." });

            var discounts = await _discountService.GetAllDiscountsAsync(null, null, page, pageSize);

            if (discounts == null || !discounts.Any())
                return NotFound(new { Message = "No discounts found for the specified criteria." });

            Log.Information("Retrieved {Count} discounts", discounts.Count());

            return Ok(discounts);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while getting discounts.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{discountId:guid}")]
    [ProducesResponseType(typeof(Discount), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Discount>> GetDiscountById(Guid discountId)
    {
        try
        {
            var discount = await _discountService.GetDiscountByIdAsync(discountId);
            if (discount == null)
            {
                Log.Information("Discount with ID {DiscountId} not found", discountId);
                return NotFound(new { Message = "Discount not found." });
            }

            Log.Information("Retrieved discount with ID {DiscountId}", discountId);
            return Ok(discount);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while getting discount by ID: {DiscountId}", discountId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AddDiscount([FromBody] Discount discount)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _discountService.AddDiscountAsync(discount);
            Log.Information("Created discount with ID {DiscountId}", discount.Id);
            return CreatedAtAction(nameof(GetDiscountById), new { discountId = discount.Id }, discount);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while creating a discount.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("applied")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AddAppliedDiscount([FromBody] AppliedDiscount discount)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _discountService.AddAppliedDiscountAsync(discount);
            Log.Information("Created applied discount with ID {DiscountId}", discount.Id);
            return Ok(new { Message = "AppliedDiscount created successfully." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while creating an applied discount.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{discountId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> UpdateDiscount(Guid discountId, [FromBody] Discount discount)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var existingDiscount = await _discountService.GetDiscountByIdAsync(discountId);
            if (existingDiscount == null)
            {
                Log.Information("Discount with ID {DiscountId} not found for update", discountId);
                return NotFound(new { Message = "Discount not found." });
            }

            await _discountService.UpdateDiscountAsync(discountId, discount);
            Log.Information("Updated discount with ID {DiscountId}", discountId);
            return Ok(new { Message = "Discount updated successfully." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while updating discount with ID: {DiscountId}", discountId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{discountId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteDiscount(Guid discountId)
    {
        try
        {
            var existingDiscount = await _discountService.GetDiscountByIdAsync(discountId);
            if (existingDiscount == null)
            {
                Log.Information("Discount with ID {DiscountId} not found for deletion", discountId);
                return NotFound(new { Message = "Discount not found." });
            }

            await _discountService.DeleteDiscountAsync(discountId);
            Log.Information("Deleted discount with ID {DiscountId}", discountId);
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while deleting discount with ID: {DiscountId}", discountId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("orders/{orderId:guid}/discount/{orderItemId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> ApplyDiscountToOrderItem(Guid orderId, Guid orderItemId, [FromQuery] Guid discountId)
    {
        try
        {
            var discount = await _discountService.GetDiscountByIdAsync(discountId);
            if (discount == null)
            {
                Log.Information("Discount with ID {DiscountId} not found", discountId);
                return NotFound(new { Message = "Discount not found." });
            }

            await _discountService.ApplyDiscountToOrderItemAsync(orderId, orderItemId, discountId);
            Log.Information("Applied discount with ID {DiscountId} to order item {OrderItemId} in order {OrderId}", discountId, orderItemId, orderId);
            return Ok(new { Message = "Discount applied successfully." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while applying discount with ID: {DiscountId} to order item {OrderItemId} in order {OrderId}", discountId, orderItemId, orderId);
            return StatusCode(500, "Internal server error");
        }
    }
}
