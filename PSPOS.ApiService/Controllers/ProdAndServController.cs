using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.Schemas;

namespace PSPOS.ApiService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class ProdAndServController : ControllerBase
    {
        private readonly IProdAndServService _service;

        public ProdAndServController(IProdAndServService service)
        {
            _service = service;
        }

        // **Products**

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (products, totalCount) = await _service.GetAllProductsSchemaAsync(from, to, page, pageSize);

            var metadata = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(metadata));

            return Ok(products);
        }

        [HttpGet("products/{productId:guid}")]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var product = await _service.GetProductSchemaByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost("products")]
        public async Task<IActionResult> AddProductSchemaAsync([FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProduct = await _service.AddProductSchemaAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { productId = createdProduct.Id }, createdProduct);
        }

        [HttpPut("products/{productId:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] ProductDTO updatedProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.UpdateProductSchemaAsync(productId, updatedProductDto);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("products/{productId:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var success = await _service.DeleteProductAsync(productId);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // **Services**

        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (services, totalCount) = await _service.GetAllServicesAsync(from, to, page, pageSize);

            var metadata = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(metadata));

            return Ok(services);
        }

        [HttpGet("services/{serviceId:guid}")]
        public async Task<IActionResult> GetServiceById(Guid serviceId)
        {
            var service = await _service.GetServiceByIdAsync(serviceId);

            if (service == null)
            {
                return NotFound();
            }

            return Ok(service);
        }

        [HttpPost("services")]
        public async Task<IActionResult> AddService([FromBody] ServiceDTO serviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdService = await _service.AddServiceAsync(serviceDto);
            return CreatedAtAction(nameof(GetServiceById), new { serviceId = createdService.Id }, createdService);
        }

        [HttpPut("services/{serviceId:guid}")]
        public async Task<IActionResult> UpdateService(Guid serviceId, [FromBody] ServiceDTO updatedServiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.UpdateServiceAsync(serviceId, updatedServiceDto);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("services/{serviceId:guid}")]
        public async Task<IActionResult> DeleteService(Guid serviceId)
        {
            var success = await _service.DeleteServiceAsync(serviceId);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
