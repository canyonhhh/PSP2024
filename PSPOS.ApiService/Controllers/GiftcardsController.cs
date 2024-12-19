using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Controllers
{
    [ApiController]
    [Route("api/giftcards")]
    public class GiftcardsController : Controller
    {
        private readonly IGiftcardService _giftcardService;

        public GiftcardsController(IGiftcardService giftcardService)
        {
            _giftcardService = giftcardService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Giftcard>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Giftcard>>> GetAllGiftcards(
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

            var giftcards = await _giftcardService.GetAllGiftcardsAsync(null, null, page, pageSize);

            if (giftcards == null || !giftcards.Any())
                return NotFound(new { Message = "No Giftcards found for the specified criteria." });

            return Ok(giftcards);
        }

        [HttpGet("{giftcardId:guid}")]
        [ProducesResponseType(typeof(Giftcard), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Giftcard>> GetGiftcardById(Guid giftcardId)
        {
            var giftcard = await _giftcardService.GetGiftcardByIdAsync(giftcardId);
            if (giftcard == null)
                return NotFound(new { Message = "Giftcard not found." });

            return Ok(giftcard);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> AddGiftcard([FromBody] Giftcard giftcard)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _giftcardService.AddGiftcardAsync(giftcard);
            return CreatedAtAction(nameof(GetGiftcardById), new { giftcardId = giftcard.Id }, giftcard);
        }

        [HttpPut("{giftcardId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateGiftcard(Guid giftcardId, [FromBody] Giftcard giftcard)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingGiftcard = await _giftcardService.GetGiftcardByIdAsync(giftcardId);
            if (existingGiftcard == null)
                return NotFound(new { Message = "giftcard not found." });

            await _giftcardService.UpdateGiftcardAsync(giftcardId, giftcard);
            return Ok(new { Message = "giftcard updated successfully." });
        }

        [HttpDelete("{giftcardId:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteGiftcard(Guid giftcardId)
        {
            var existinggiftcard = await _giftcardService.GetGiftcardByIdAsync(giftcardId);
            if (existinggiftcard == null)
                return NotFound(new { Message = "giftcard not found." });

            await _giftcardService.DeleteGiftcardAsync(giftcardId);
            return NoContent();
        }
    }
}
