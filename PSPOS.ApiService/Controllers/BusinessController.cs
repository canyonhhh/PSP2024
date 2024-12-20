using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using Serilog;

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
        try
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

            Log.Information("Retrieved {TotalCount} businesses", totalCount);

            return Ok(businesses);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while getting businesses.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{businessId:guid}")]
    public async Task<IActionResult> GetBusinessById(Guid businessId)
    {
        try
        {
            var business = await _businessService.GetBusinessByIdAsync(businessId);

            if (business == null)
            {
                Log.Information("Business with ID {BusinessId} not found", businessId);
                return NotFound();
            }

            Log.Information("Retrieved business with ID {BusinessId}", businessId);
            return Ok(business);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while getting business by ID: {BusinessId}", businessId);
            return StatusCode(500, "Internal server error");
        }
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
            Log.Information("Created business with ID {BusinessId}", createdBusiness.Id);
            return CreatedAtAction(nameof(GetBusinessById), new { businessId = createdBusiness.Id }, createdBusiness);
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, "An error occurred while creating a business.");
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unexpected error occurred while creating a business.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{businessId:guid}")]
    public async Task<IActionResult> UpdateBusiness(Guid businessId, [FromBody] Business updatedBusiness)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _businessService.UpdateBusinessAsync(businessId, updatedBusiness);

            if (result == null)
            {
                Log.Information("Business with ID {BusinessId} not found for update", businessId);
                return NotFound();
            }

            Log.Information("Updated business with ID {BusinessId}", businessId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while updating business with ID: {BusinessId}", businessId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{businessId:guid}")]
    public async Task<IActionResult> DeleteBusiness(Guid businessId)
    {
        try
        {
            var success = await _businessService.DeleteBusinessAsync(businessId);

            if (!success)
            {
                Log.Information("Business with ID {BusinessId} not found for deletion", businessId);
                return NotFound();
            }

            Log.Information("Deleted business with ID {BusinessId}", businessId);
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while deleting business with ID: {BusinessId}", businessId);
            return StatusCode(500, "Internal server error");
        }
    }
}
