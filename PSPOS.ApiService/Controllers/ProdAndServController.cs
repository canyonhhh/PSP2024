using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Controllers
{
    [ApiController]
    [Route("api")]
    public class ProdAndServController : ControllerBase
    {
        private readonly ProdAndServService _service;

        public ProdAndServController(ProdAndServService service)
        {
            _service = service;
        }

        // **Products**

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("products/{productId}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("products")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            await _service.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("products/{productId}")]
        public async Task<IActionResult> UpdateProduct(Guid id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            await _service.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("products/{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _service.DeleteProductAsync(id);
            return NoContent();
        }

        // **Services**

        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _service.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("services/{serviceId}")]
        public async Task<IActionResult> GetServiceById(Guid id)
        {
            var service = await _service.GetServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return Ok(service);
        }

        [HttpPost("services")]
        public async Task<IActionResult> AddService(Service service)
        {
            await _service.AddServiceAsync(service);
            return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, service);
        }

        [HttpPut("services/{serviceId}")]
        public async Task<IActionResult> UpdateService(Guid id, Service service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            await _service.UpdateServiceAsync(service);
            return NoContent();
        }

        [HttpDelete("services/{serviceId}")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            await _service.DeleteServiceAsync(id);
            return NoContent();
        }
    }
}
