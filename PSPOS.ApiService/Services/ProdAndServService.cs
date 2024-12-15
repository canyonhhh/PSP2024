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
            if (updatedProduct == null)
            {
                return null;
            }

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

            await _repository.RemoveProductFromGroupAsync(id);

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
            await _repository.RemoveServiceFromGroupAsync(id);

            return await _repository.DeleteServiceAsync(id);
        }

        // **Categories**

        public async Task<(IEnumerable<ProductCategorySchema> Categories, int TotalCount)> GetAllCategoriesAsync(int skip = 0, int limit = 10)
        {
            var (productGroups, productCount) = await _repository.GetAllProductGroupsAsync(skip, limit);
            var (serviceGroups, serviceCount) = await _repository.GetAllServiceGroupsAsync(skip, limit);

            var categories = productGroups
                .Select(pg => new ProductCategorySchema
                {
                    Id = pg.Id,
                    CreatedAt = pg.CreatedAt,
                    UpdatedAt = pg.UpdatedAt,
                    CreatedBy = pg.CreatedBy,
                    UpdatedBy = pg.UpdatedBy,
                    name = pg.Name,
                    description = pg.Description,
                    productOrServiceIds = pg.productOrServiceIds
                })
                .Concat(serviceGroups.Select(sg => new ProductCategorySchema
                {
                    Id = sg.Id,
                    CreatedAt = sg.CreatedAt,
                    UpdatedAt = sg.UpdatedAt,
                    CreatedBy = sg.CreatedBy,
                    UpdatedBy = sg.UpdatedBy,
                    name = sg.Name,
                    description = sg.Description,
                    productOrServiceIds = sg.productOrServiceIds
                }))
                .ToList();

            return (categories, productCount + serviceCount);
        }

        public async Task<ProductCategorySchema?> GetCategoryByIdAsync(Guid categoryId)
        {
            var productGroup = await _repository.GetProductGroupByIdAsync(categoryId);
            if (productGroup != null)
            {
                return new ProductCategorySchema
                {
                    Id = productGroup.Id,
                    CreatedAt = productGroup.CreatedAt,
                    UpdatedAt = productGroup.UpdatedAt,
                    CreatedBy = productGroup.CreatedBy,
                    UpdatedBy = productGroup.UpdatedBy,
                    name = productGroup.Name,
                    description = productGroup.Description,
                    productOrServiceIds = productGroup.productOrServiceIds
                };
            }

            var serviceGroup = await _repository.GetServiceGroupByIdAsync(categoryId);
            if (serviceGroup != null)
            {
                return new ProductCategorySchema
                {
                    Id = serviceGroup.Id,
                    CreatedAt = serviceGroup.CreatedAt,
                    UpdatedAt = serviceGroup.UpdatedAt,
                    CreatedBy = serviceGroup.CreatedBy,
                    UpdatedBy = serviceGroup.UpdatedBy,
                    name = serviceGroup.Name,
                    description = serviceGroup.Description,
                    productOrServiceIds = serviceGroup.productOrServiceIds
                };
            }

            return null;
        }

        public async Task<ProductCategorySchema> AddCategoryAsync(CategoryDTO categoryDto)
        {
            if (categoryDto.ProductOrServiceIds.Length == 0)
            {
                throw new ArgumentException("Category must include product or service IDs.");
            }

            var allProducts = true;
            var allServices = true;

            foreach (var id in categoryDto.ProductOrServiceIds)
            {
                if (!await _repository.IsProductAsync(id))
                {
                    allProducts = false;
                }
                if (!await _repository.IsServiceAsync(id))
                {
                    allServices = false;
                }
            }

            if (allProducts && !allServices)
            {
                var productGroup = new ProductGroup(categoryDto.Name, categoryDto.Description, categoryDto.ProductOrServiceIds);
                var addedGroup = await _repository.AddProductGroupAsync(productGroup);

                return new ProductCategorySchema
                {
                    Id = addedGroup.Id,
                    CreatedAt = addedGroup.CreatedAt,
                    UpdatedAt = addedGroup.UpdatedAt,
                    CreatedBy = addedGroup.CreatedBy,
                    UpdatedBy = addedGroup.UpdatedBy,
                    name = addedGroup.Name,
                    description = addedGroup.Description,
                    productOrServiceIds = addedGroup.productOrServiceIds
                };
            }
            else if (!allProducts && allServices)
            {
                var serviceGroup = new ServiceGroup(categoryDto.Name, categoryDto.Description, categoryDto.ProductOrServiceIds);
                var addedServiceGroup = await _repository.AddServiceGroupAsync(serviceGroup);

                return new ProductCategorySchema
                {
                    Id = addedServiceGroup.Id,
                    CreatedAt = addedServiceGroup.CreatedAt,
                    UpdatedAt = addedServiceGroup.UpdatedAt,
                    CreatedBy = addedServiceGroup.CreatedBy,
                    UpdatedBy = addedServiceGroup.UpdatedBy,
                    name = addedServiceGroup.Name,
                    description = addedServiceGroup.Description,
                    productOrServiceIds = addedServiceGroup.productOrServiceIds
                };
            }
            else
            {
                throw new ArgumentException("A category must consist of only services or products.");
            }
        }


        public async Task<ProductCategorySchema?> UpdateCategoryAsync(Guid categoryId, CategoryDTO categoryDto)
        {
            var productGroup = await _repository.GetProductGroupByIdAsync(categoryId);
            if (productGroup != null)
            {
                // Check if the product group is empty
                if (productGroup.productOrServiceIds == null || productGroup.productOrServiceIds.Length == 0)
                {
                    // Allow adding either products or services
                    foreach (var id in categoryDto.ProductOrServiceIds)
                    {
                        if (!await _repository.IsProductAsync(id) && !await _repository.IsServiceAsync(id))
                        {
                            throw new InvalidOperationException("Invalid ID. It must be either a product or a service.");
                        }
                    }
                }
                else
                {
                    // Validate that all IDs are products
                    foreach (var id in categoryDto.ProductOrServiceIds)
                    {
                        if (!await _repository.IsProductAsync(id))
                        {
                            throw new InvalidOperationException("Cannot add a service to a product group.");
                        }
                    }
                }

                // Update the product group
                productGroup.Name = categoryDto.Name;
                productGroup.Description = categoryDto.Description;
                productGroup.productOrServiceIds = categoryDto.ProductOrServiceIds;

                var updatedGroup = await _repository.UpdateProductGroupAsync(productGroup);
                if (updatedGroup != null)
                {
                    return new ProductCategorySchema
                    {
                        Id = updatedGroup.Id,
                        CreatedAt = updatedGroup.CreatedAt,
                        UpdatedAt = updatedGroup.UpdatedAt,
                        CreatedBy = updatedGroup.CreatedBy,
                        UpdatedBy = updatedGroup.UpdatedBy,
                        name = updatedGroup.Name,
                        description = updatedGroup.Description,
                        productOrServiceIds = categoryDto.ProductOrServiceIds
                    };
                }
            }

            var serviceGroup = await _repository.GetServiceGroupByIdAsync(categoryId);
            if (serviceGroup != null)
            {
                // Check if the service group is empty
                if (serviceGroup.productOrServiceIds == null || serviceGroup.productOrServiceIds.Length == 0)
                {
                    // Allow adding either products or services
                    foreach (var id in categoryDto.ProductOrServiceIds)
                    {
                        if (!await _repository.IsProductAsync(id) && !await _repository.IsServiceAsync(id))
                        {
                            throw new InvalidOperationException("Invalid ID. It must be either a product or a service.");
                        }
                    }
                }
                else
                {
                    // Validate that all IDs are services
                    foreach (var id in categoryDto.ProductOrServiceIds)
                    {
                        if (!await _repository.IsServiceAsync(id))
                        {
                            throw new InvalidOperationException("Cannot add a product to a service group.");
                        }
                    }
                }

                // Update the service group
                serviceGroup.Name = categoryDto.Name;
                serviceGroup.Description = categoryDto.Description;
                serviceGroup.productOrServiceIds = categoryDto.ProductOrServiceIds;

                var updatedGroup = await _repository.UpdateServiceGroupAsync(serviceGroup);
                if (updatedGroup != null)
                {
                    return new ProductCategorySchema
                    {
                        Id = updatedGroup.Id,
                        CreatedAt = updatedGroup.CreatedAt,
                        UpdatedAt = updatedGroup.UpdatedAt,
                        CreatedBy = updatedGroup.CreatedBy,
                        UpdatedBy = updatedGroup.UpdatedBy,
                        name = updatedGroup.Name,
                        description = updatedGroup.Description,
                        productOrServiceIds = categoryDto.ProductOrServiceIds
                    };
                }
            }

            return null;
        }

        public async Task<bool> DeleteCategoryAsync(Guid categoryId)
        {
            var productGroup = await _repository.GetProductGroupByIdAsync(categoryId);
            if (productGroup != null)
            {
                return await _repository.DeleteProductGroupAsync(categoryId);
            }

            var serviceGroup = await _repository.GetServiceGroupByIdAsync(categoryId);
            if (serviceGroup != null)
            {
                return await _repository.DeleteServiceGroupAsync(categoryId);
            }

            return false;
        }

    }
}