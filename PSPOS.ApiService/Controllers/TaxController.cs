using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Controllers
{
    [ApiController]
    [Route("tax")]
    [Produces("application/json")]
    public class TaxController : ControllerBase
    {
        private readonly ITaxService _taxService;
        public TaxController(ITaxService taxService)
        {
            _taxService = taxService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Tax>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Tax>>> GetTaxes(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var allTaxes = await _taxService.GetAllTaxesAsync();
            var query = allTaxes.AsQueryable();

            if (from.HasValue)
                query = query.Where(t => t.CreatedDate >= from.Value);

            if (to.HasValue)
                query = query.Where(t => t.CreatedDate <= to.Value);

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (!items.Any()) return NotFound();

            return Ok(items);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Tax), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateTax([FromBody] Tax tax)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _taxService.AddTaxAsync(tax);
            return Ok(tax);
        }

        [HttpGet("{taxId}")]
        [ProducesResponseType(typeof(Tax), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Tax>> GetTaxById(Guid taxId)
        {
            var tax = await _taxService.GetTaxByIdAsync(taxId);
            if (tax == null) return NotFound();
            return Ok(tax);
        }

        [HttpPut("{taxId}")]
        [ProducesResponseType(typeof(Tax), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateTax(Guid taxId, [FromBody] Tax tax)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (taxId != tax.Id) return BadRequest("Tax ID mismatch.");

            var existing = await _taxService.GetTaxByIdAsync(taxId);
            if (existing == null) return NotFound("Tax not found.");

            await _taxService.UpdateTaxAsync(taxId, tax);
            return Ok(tax);
        }

        [HttpDelete("{taxId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteTax(Guid taxId)
        {
            var existing = await _taxService.GetTaxByIdAsync(taxId);
            if (existing == null) return NotFound("Tax not found.");

            await _taxService.DeleteTaxAsync(taxId);
            return NoContent();
        }

    }
}
