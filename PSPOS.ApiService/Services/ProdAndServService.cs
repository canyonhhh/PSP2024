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

        public async Task<(IEnumerable<ProductSchema> ProductsSchema, int TotalCount)> GetAllProductsAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
        {
            var (products, totalCount) = await _repository.GetAllProductsAsync(from, to, page, pageSize);
            var productSchemas = new List<ProductSchema>();

            foreach (var product in products)
            {
                var productStock = await _repository.GetProductStockAsync(product.Id);
                if (productStock != null)
                {
                    productSchemas.Add(new ProductSchema
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
                    });
                }
            }

            return (productSchemas, totalCount);
        }

        public async Task<(IEnumerable<ProductSchema> ProductsSchema, int TotalCount)> GetAllProductsSchemaAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
        {
            var (products, totalCount) = await _repository.GetAllProductsAsync(from, to, page, pageSize);
            var productSchemas = new List<ProductSchema>();

            foreach (var product in products)
            {
                var productStock = await _repository.GetProductStockAsync(product.Id);
                if (productStock != null)
                {
                    productSchemas.Add(new ProductSchema
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
                    });
                }
            }

            return (productSchemas, totalCount);
        }

        public async Task<ProductSchema?> GetProductByIdAsync(Guid id)
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

        public async Task<ProductSchema> AddProductSchemaAsync(ProductDTO productDto)
        {
            var product = new Product(
                productDto.Name,
                productDto.Description,
                productDto.Price,
                productDto.ImageUrl,
                productDto.BusinessId,
                productDto.BaseProductId
            );

            var addedProduct = await _repository.AddProductAsync(product);
            var productStock = new ProductStock(productDto.StockQuantity, addedProduct.Id);

            await _repository.AddProductStockAsync(productStock);

            return new ProductSchema
            {
                Id = addedProduct.Id,
                CreatedAt = addedProduct.CreatedAt,
                UpdatedAt = addedProduct.UpdatedAt,
                CreatedBy = addedProduct.CreatedBy,
                UpdatedBy = addedProduct.UpdatedBy,
                name = addedProduct.Name,
                description = addedProduct.Description,
                price = addedProduct.Price,
                imageUrl = addedProduct.ImageUrl,
                stockQuantity = productStock.Quantity,
                businessId = addedProduct.BusinessId,
                baseProductId = addedProduct.BaseProductId,
            };
        }

        public async Task<ProductSchema?> UpdateProductSchemaAsync(Guid productId, ProductDTO updatedProductDto)
        {
            var existingProduct = await _repository.GetProductByIdAsync(productId);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = updatedProductDto.Name;
            existingProduct.Description = updatedProductDto.Description;
            existingProduct.Price = updatedProductDto.Price;
            existingProduct.ImageUrl = updatedProductDto.ImageUrl;
            existingProduct.BusinessId = updatedProductDto.BusinessId;
            existingProduct.BaseProductId = updatedProductDto.BaseProductId;

            var updatedProduct = await _repository.UpdateProductAsync(productId, existingProduct);
            var productStock = await _repository.GetProductStockAsync(productId);
            if (productStock != null)
            {
                productStock.Quantity = updatedProductDto.StockQuantity;
                await _repository.UpdateProductStockAsync(productStock);
            }

            return new ProductSchema
            {
                Id = updatedProduct.Id,
                CreatedAt = updatedProduct.CreatedAt,
                UpdatedAt = updatedProduct.UpdatedAt,
                CreatedBy = updatedProduct.CreatedBy,
                UpdatedBy = updatedProduct.UpdatedBy,
                name = updatedProduct.Name,
                description = updatedProduct.Description,
                price = updatedProduct.Price,
                imageUrl = updatedProduct.ImageUrl,
                stockQuantity = productStock?.Quantity ?? 0,
                businessId = updatedProduct.BusinessId,
                baseProductId = updatedProduct.BaseProductId,
            };
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _repository.GetProductByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            var productStock = await _repository.GetProductStockAsync(id);
            if (productStock != null)
            {
                await _repository.DeleteProductStockAsync(productStock);
            }

            return await _repository.DeleteProductAsync(id);
        }

        // **Services**

        public async Task<(IEnumerable<ServiceSchema> ServicesSchema, int TotalCount)> GetAllServicesAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
        {
            var (services, totalCount) = await _repository.GetAllServicesAsync(from, to, page, pageSize);
            var serviceSchemas = services.Select(service => new ServiceSchema
            {
                Id = service.Id,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt,
                CreatedBy = service.CreatedBy,
                UpdatedBy = service.UpdatedBy,
                name = service.Name,
                description = service.Description,
                price = service.Price,
                interval = service.Interval,
                employeeId = service.EmployeeId
            }).ToList();

            return (serviceSchemas, totalCount);
        }

        public async Task<ServiceSchema?> GetServiceByIdAsync(Guid id)
        {
            var service = await _repository.GetServiceByIdAsync(id);
            if (service == null)
            {
                return null;
            }

            return new ServiceSchema
            {
                Id = service.Id,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt,
                CreatedBy = service.CreatedBy,
                UpdatedBy = service.UpdatedBy,
                name = service.Name,
                description = service.Description,
                price = service.Price,
                interval = service.Interval,
                employeeId = service.EmployeeId
            };
        }

        public async Task<ServiceSchema> AddServiceAsync(ServiceDTO serviceDto)
        {
            var service = new Service(
                serviceDto.Name,
                serviceDto.Description,
                serviceDto.Price,
                serviceDto.Duration,
                serviceDto.EmployeeId
            );

            var addedService = await _repository.AddServiceAsync(service);

            return new ServiceSchema
            {
                Id = addedService.Id,
                CreatedAt = addedService.CreatedAt,
                UpdatedAt = addedService.UpdatedAt,
                CreatedBy = addedService.CreatedBy,
                UpdatedBy = addedService.UpdatedBy,
                name = addedService.Name,
                description = addedService.Description,
                price = addedService.Price,
                interval = addedService.Interval,
                employeeId = addedService.EmployeeId
            };
        }

        public async Task<Service?> UpdateServiceAsync(Guid serviceId, ServiceDTO updatedServiceDto)
        {
            var existingService = await _repository.GetServiceByIdAsync(serviceId);
            if (existingService == null)
            {
                return null;
            }

            existingService.Name = updatedServiceDto.Name;
            existingService.Description = updatedServiceDto.Description;
            existingService.Price = updatedServiceDto.Price;
            existingService.Interval = updatedServiceDto.Duration;
            existingService.EmployeeId = updatedServiceDto.EmployeeId;

            return await _repository.UpdateServiceAsync(serviceId, existingService);
        }

        public async Task<bool> DeleteServiceAsync(Guid id)
        {
            return await _repository.DeleteServiceAsync(id);
        }
    }
}
