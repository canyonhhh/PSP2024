using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using Serilog;

namespace PSPOS.ApiService.Controllers
{
    [ApiController]
    [Route("api")]
    public class ProdAndServController : ControllerBase
    {
        private readonly IProdAndServService _service;

        public ProdAndServController(IProdAndServService service)
        {
            _service = service;
        }

        // **Categories**

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] int skip = 0, [FromQuery] int limit = 10)
        {
            Log.Information("Fetching categories with skip: {Skip}, limit: {Limit}", skip, limit);
            var (categories, totalCount) = await _service.GetAllCategoriesAsync(skip, limit);
            return Ok(new { categories, totalCount });
        }

        [HttpGet("categories/{id:guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            Log.Information("Fetching category with ID: {Id}", id);
            var category = await _service.GetCategoryByIdAsync(id);
            if (category == null)
            {
                Log.Warning("Category not found with ID: {Id}", id);
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost("categories")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDto)
        {
            Log.Information("Adding new category with name: {Name}", categoryDto.Name);
            try
            {
                var category = await _service.AddCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (ArgumentException ex)
            {
                Log.Error("Error adding category: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("categories/{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryDTO categoryDto)
        {
            Log.Information("Updating category with ID: {Id}", id);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _service.UpdateCategoryAsync(id, categoryDto);

                if (result == null)
                {
                    Log.Warning("Category not found with ID: {Id}", id);
                    return NotFound();
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error("Error updating category: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                Log.Error("Error updating category: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("categories/{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            Log.Information("Deleting category with ID: {Id}", id);
            var success = await _service.DeleteCategoryAsync(id);

            if (!success)
            {
                Log.Warning("Category not found with ID: {Id}", id);
                return NotFound();
            }

            return NoContent();
        }

        // **Products**

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            Log.Information("Fetching products from: {From}, to: {To}, page: {Page}, pageSize: {PageSize}", from, to, page, pageSize);
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
            Log.Information("Fetching product with ID: {ProductId}", productId);
            var product = await _service.GetProductSchemaByIdAsync(productId);

            if (product == null)
            {
                Log.Warning("Product not found with ID: {ProductId}", productId);
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost("products")]
        public async Task<IActionResult> AddProductSchemaAsync([FromBody] ProductDTO productDto)
        {
            Log.Information("Adding new product with name: {Name}", productDto.Name);
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
            Log.Information("Updating product with ID: {ProductId}", productId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.UpdateProductSchemaAsync(productId, updatedProductDto);

            if (result == null)
            {
                Log.Warning("Product not found with ID: {ProductId}", productId);
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("products/{productId:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            Log.Information("Deleting product with ID: {ProductId}", productId);
            var success = await _service.DeleteProductAsync(productId);

            if (!success)
            {
                Log.Warning("Product not found with ID: {ProductId}", productId);
                return NotFound();
            }

            return NoContent();
        }

        // **Services**

        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            Log.Information("Fetching services from: {From}, to: {To}, page: {Page}, pageSize: {PageSize}", from, to, page, pageSize);
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
            Log.Information("Fetching service with ID: {ServiceId}", serviceId);
            var service = await _service.GetServiceByIdAsync(serviceId);

            if (service == null)
            {
                Log.Warning("Service not found with ID: {ServiceId}", serviceId);
                return NotFound();
            }

            return Ok(service);
        }

        [HttpPost("services")]
        public async Task<IActionResult> AddService([FromBody] ServiceDTO serviceDto)
        {
            Log.Information("Adding new service with name: {Name}", serviceDto.Name);
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
            Log.Information("Updating service with ID: {ServiceId}", serviceId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.UpdateServiceAsync(serviceId, updatedServiceDto);

            if (result == null)
            {
                Log.Warning("Service not found with ID: {ServiceId}", serviceId);
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("services/{serviceId:guid}")]
        public async Task<IActionResult> DeleteService(Guid serviceId)
        {
            Log.Information("Deleting service with ID: {ServiceId}", serviceId);
            var success = await _service.DeleteServiceAsync(serviceId);

            if (!success)
            {
                Log.Warning("Service not found with ID: {ServiceId}", serviceId);
                return NotFound();
            }

            return NoContent();
        }
    }
}