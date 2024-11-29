using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountService _discountService;

    public DiscountsController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Discount>>> GetAllDiscounts()
    {
        var discounts = await _discountService.GetAllDiscountsAsync();
        return Ok(discounts);
    }

    [HttpGet("{discountId}")]
    public async Task<ActionResult<Discount>> GetDiscountById(Guid discountId)
    {
        var discount = await _discountService.GetDiscountByIdAsync(discountId);
        if (discount == null)
            return NotFound();

        return Ok(discount);
    }

    [HttpPost]
    public async Task<ActionResult> AddDiscount([FromBody] Discount discount)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _discountService.AddDiscountAsync(discount);
        return Ok();
    }

    [HttpPut("{discountId}")]
    public async Task<ActionResult> UpdateDiscount(Guid discountId, [FromBody] Discount discount)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _discountService.UpdateDiscountAsync(discountId, discount);
        return Ok();
    }

    [HttpDelete("{discountId}")]
    public async Task<ActionResult> DeleteDiscount(Guid discountId)
    {
        await _discountService.DeleteDiscountAsync(discountId);
        return NoContent();
    }

    [HttpPost("/orders/{orderId}/discount/{orderItemId}")]
    public async Task<ActionResult> ApplyDiscountToOrderItem(Guid orderId, Guid orderItemId, Guid discountId)
    {
        await _discountService.ApplyDiscountToOrderItemAsync(orderId, orderItemId, discountId);
        return Ok();
    }
}
