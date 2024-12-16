using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

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
        if (page <= 0 || pageSize <= 0)
            return BadRequest(new { Message = "Page and pageSize must be positive integers." });

        if (from == null && !string.IsNullOrEmpty(from))
            return BadRequest(new { Message = "Invalid 'from' date format. Use ISO 8601 (UTC)." });

        if (to == null && !string.IsNullOrEmpty(to))
            return BadRequest(new { Message = "Invalid 'to' date format. Use ISO 8601 (UTC)." });

        var discounts = await _discountService.GetAllDiscountsAsync(null, null, page, pageSize);

        if (discounts == null || !discounts.Any())
            return NotFound(new { Message = "No discounts found for the specified criteria." });

        return Ok(discounts);
    }

    [HttpGet("{discountId:guid}")]
    [ProducesResponseType(typeof(Discount), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Discount>> GetDiscountById(Guid discountId)
    {
        var discounts = await _discountService.GetDiscountByIdAsync(discountId);
        if (discounts == null)
            return NotFound(new { Message = "Discount not found." });

        return Ok(discounts);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AddDiscount([FromBody] Discount discount)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _discountService.AddDiscountAsync(discount);
        return CreatedAtAction(nameof(GetDiscountById), new { discountId = discount.Id }, discount);
    }
    [HttpPost("applied")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AddAppliedDiscount([FromBody] AppliedDiscount discount)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _discountService.AddAppliedDiscountAsync(discount);
        return Ok(new { Message = "AppliedDiscount created successfully." });
    }

    [HttpPut("{discountId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> UpdateDiscount(Guid discountId, [FromBody] Discount discount)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingDiscounts = await _discountService.GetDiscountByIdAsync(discountId);
        if (existingDiscounts == null)
            return NotFound(new { Message = "Discount not found." });

        await _discountService.UpdateDiscountAsync(discountId, discount);
        return Ok(new { Message = "Discount updated successfully." });
    }

    [HttpDelete("{discountId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteDiscount(Guid discountId)
    {
        var existingDiscount = await _discountService.GetDiscountByIdAsync(discountId);
        if (existingDiscount == null)
            return NotFound(new { Message = "Discount not found." });

        await _discountService.DeleteDiscountAsync(discountId);
        return NoContent();
    }

    [HttpPost("orders/{orderId:guid}/discount/{orderItemId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> ApplyDiscountToOrderItem(Guid orderId, Guid orderItemId, [FromQuery] Guid discountId)
    {
        var discount = await _discountService.GetDiscountByIdAsync(discountId);
        if (discount == null)
            return NotFound(new { Message = "Discount not found." });

        await _discountService.ApplyDiscountToOrderItemAsync(orderId, orderItemId, discountId);
        return Ok(new { Message = "Discount applied successfully." });
    }

}