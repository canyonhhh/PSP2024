using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.Schemas;

namespace PSPOS.ApiService.Services
{
    public class ProdAndServService : IProdAndServService
    {
        private readonly IProdAndServRepository _repository;

        public ProdAndServService(IProdAndServRepository repository)
        {
            _repository = repository;
        }

        // **Products**

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllProductsAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
        {
            return await _repository.GetAllProductsAsync(from, to, page, pageSize);
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _repository.GetProductByIdAsync(id);
        }

        public async Task<ProductSchema?> GetProductSchemaByIdAsync(Guid id)
        {
            Product? product = await _repository.GetProductByIdAsync(id);
            if (product == null)
            {
                return null;
            }

            ProductStock? productStock = await _repository.GetProductStockAsync(id);
            if (productStock == null) 
            {
                return null;
            }

            return new()
            {
                Id = product.Id,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                CreatedBy = product.CreatedBy,
                UpdatedBy = product.UpdatedBy,
                name = product.Name,
                description = product.Description,
                price = product.Price,
                imageUrl = product.ImageUrl,
                stockQuantity = productStock.Quantity,
                businessId = product.BusinessId,
                baseProductId = product.BaseProductId,
            };
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            return await _repository.AddProductAsync(product);
        }

        public async Task<Product?> UpdateProductAsync(Guid productId, Product updatedProduct)
        {
            return await _repository.UpdateProductAsync(productId, updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            return await _repository.DeleteProductAsync(id);
        }

        // **Services**

        public async Task<(IEnumerable<Service> Services, int TotalCount)> GetAllServicesAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
        {
            return await _repository.GetAllServicesAsync(from, to, page, pageSize);
        }

        public async Task<Service?> GetServiceByIdAsync(Guid id)
        {
            return await _repository.GetServiceByIdAsync(id);
        }

        public async Task<Service> AddServiceAsync(Service service)
        {
            return await _repository.AddServiceAsync(service);
        }

        public async Task<Service?> UpdateServiceAsync(Guid serviceId, Service updatedService)
        {
            return await _repository.UpdateServiceAsync(serviceId, updatedService);
        }

        public async Task<bool> DeleteServiceAsync(Guid id)
        {
            return await _repository.DeleteServiceAsync(id);
        }
    }
}
