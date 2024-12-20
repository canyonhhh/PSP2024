using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using Serilog;

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
            try
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

                Log.Information("Retrieved {Count} giftcards", giftcards.Count());
                return Ok(giftcards);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting giftcards.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{giftcardId:guid}")]
        [ProducesResponseType(typeof(Giftcard), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Giftcard>> GetGiftcardById(Guid giftcardId)
        {
            try
            {
                var giftcard = await _giftcardService.GetGiftcardByIdAsync(giftcardId);
                if (giftcard == null)
                {
                    Log.Information("Giftcard with ID {GiftcardId} not found", giftcardId);
                    return NotFound(new { Message = "Giftcard not found." });
                }

                Log.Information("Retrieved giftcard with ID {GiftcardId}", giftcardId);
                return Ok(giftcard);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting giftcard by ID: {GiftcardId}", giftcardId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> AddGiftcard([FromBody] Giftcard giftcard)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _giftcardService.AddGiftcardAsync(giftcard);
                Log.Information("Created giftcard with ID {GiftcardId}", giftcard.Id);
                return CreatedAtAction(nameof(GetGiftcardById), new { giftcardId = giftcard.Id }, giftcard);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating a giftcard.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{giftcardId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateGiftcard(Guid giftcardId, [FromBody] Giftcard giftcard)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingGiftcard = await _giftcardService.GetGiftcardByIdAsync(giftcardId);
                if (existingGiftcard == null)
                {
                    Log.Information("Giftcard with ID {GiftcardId} not found for update", giftcardId);
                    return NotFound(new { Message = "Giftcard not found." });
                }

                await _giftcardService.UpdateGiftcardAsync(giftcardId, giftcard);
                Log.Information("Updated giftcard with ID {GiftcardId}", giftcardId);
                return Ok(new { Message = "Giftcard updated successfully." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating giftcard with ID: {GiftcardId}", giftcardId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{giftcardId:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteGiftcard(Guid giftcardId)
        {
            try
            {
                var existingGiftcard = await _giftcardService.GetGiftcardByIdAsync(giftcardId);
                if (existingGiftcard == null)
                {
                    Log.Information("Giftcard with ID {GiftcardId} not found for deletion", giftcardId);
                    return NotFound(new { Message = "Giftcard not found." });
                }

                await _giftcardService.DeleteGiftcardAsync(giftcardId);
                Log.Information("Deleted giftcard with ID {GiftcardId}", giftcardId);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting giftcard with ID: {GiftcardId}", giftcardId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}