using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using Serilog;

namespace PSPOS.ApiService.Controllers
{
    [ApiController]
    [Route("api/tax")]
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
            Log.Information("Fetching taxes from {From} to {To} with page {Page} and pageSize {PageSize}", from, to, page, pageSize);
            var allTaxes = await _taxService.GetAllTaxesAsync();
            var query = allTaxes.AsQueryable();

            if (from.HasValue)
                query = query.Where(t => t.CreatedDate >= from.Value);

            if (to.HasValue)
                query = query.Where(t => t.CreatedDate <= to.Value);

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (!items.Any())
            {
                Log.Warning("No taxes found for the given criteria.");
                return NotFound();
            }

            Log.Information("Returning {Count} taxes", items.Count);
            return Ok(items);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Tax), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateTax([FromBody] Tax tax)
        {
            Log.Information("Creating a new tax with name: {Name}", tax.Name);
            if (!ModelState.IsValid)
            {
                Log.Warning("Invalid model state for tax creation.");
                return BadRequest(ModelState);
            }
            await _taxService.AddTaxAsync(tax);
            Log.Information("Tax created successfully with ID: {Id}", tax.Id);
            return Ok(tax);
        }

        [HttpGet("{taxId}")]
        [ProducesResponseType(typeof(Tax), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Tax>> GetTaxById(Guid taxId)
        {
            Log.Information("Fetching tax with ID: {TaxId}", taxId);
            var tax = await _taxService.GetTaxByIdAsync(taxId);
            if (tax == null)
            {
                Log.Warning("Tax with ID: {TaxId} not found.", taxId);
                return NotFound();
            }
            Log.Information("Returning tax with ID: {TaxId}", taxId);
            return Ok(tax);
        }

        [HttpPut("{taxId}")]
        [ProducesResponseType(typeof(Tax), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateTax(Guid taxId, [FromBody] Tax tax)
        {
            Log.Information("Updating tax with ID: {TaxId}", taxId);
            if (!ModelState.IsValid)
            {
                Log.Warning("Invalid model state for tax update.");
                return BadRequest(ModelState);
            }
            if (taxId != tax.Id)
            {
                Log.Warning("Tax ID mismatch. Provided ID: {ProvidedId}, Tax ID: {TaxId}", taxId, tax.Id);
                return BadRequest("Tax ID mismatch.");
            }

            var existing = await _taxService.GetTaxByIdAsync(taxId);
            if (existing == null)
            {
                Log.Warning("Tax with ID: {TaxId} not found.", taxId);
                return NotFound("Tax not found.");
            }

            await _taxService.UpdateTaxAsync(taxId, tax);
            Log.Information("Tax with ID: {TaxId} updated successfully.", taxId);
            return Ok(tax);
        }

        [HttpDelete("{taxId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteTax(Guid taxId)
        {
            Log.Information("Deleting tax with ID: {TaxId}", taxId);
            var existing = await _taxService.GetTaxByIdAsync(taxId);
            if (existing == null)
            {
                Log.Warning("Tax with ID: {TaxId} not found.", taxId);
                return NotFound("Tax not found.");
            }

            await _taxService.DeleteTaxAsync(taxId);
            Log.Information("Tax with ID: {TaxId} deleted successfully.", taxId);
            return NoContent();
        }

    }
}