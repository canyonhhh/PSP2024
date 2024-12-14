using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Controllers;

[ApiController]
[Route("api/businesses")]
public class BusinessController : ControllerBase
{
    private readonly IBusinessService _businessService;

    public BusinessController(IBusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBusinesses([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (businesses, totalCount) = await _businessService.GetBusinessesAsync(from, to, page, pageSize);

        // Pagination metadata
        var metadata = new
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(metadata));

        return Ok(businesses);
    }

    [HttpGet("{businessId:guid}")]
    public async Task<IActionResult> GetBusinessById(Guid businessId)
    {
        var business = await _businessService.GetBusinessByIdAsync(businessId);

        if (business == null)
        {
            return NotFound();
        }

        return Ok(business);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBusiness([FromBody] Business business)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdBusiness = await _businessService.CreateBusinessAsync(business);
            return CreatedAtAction(nameof(GetBusinessById), new { businessId = createdBusiness.Id }, createdBusiness);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    [HttpPut("{businessId:guid}")]
    public async Task<IActionResult> UpdateBusiness(Guid businessId, [FromBody] Business updatedBusiness)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _businessService.UpdateBusinessAsync(businessId, updatedBusiness);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{businessId:guid}")]
    public async Task<IActionResult> DeleteBusiness(Guid businessId)
    {
        var success = await _businessService.DeleteBusinessAsync(businessId);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}