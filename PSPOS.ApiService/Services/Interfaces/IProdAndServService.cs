using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.Schemas;

namespace PSPOS.ApiService.Services.Interfaces
{
    public interface IProdAndServService
    {
        // **Products**
        Task<(IEnumerable<ProductSchema> ProductsSchema, int TotalCount)> GetAllProductsSchemaAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10);
        Task<ProductSchema?> GetProductSchemaByIdAsync(Guid id);
        Task<ProductSchema> AddProductSchemaAsync(ProductDTO productDto);
        Task<ProductSchema?> UpdateProductSchemaAsync(Guid productId, ProductDTO updatedProductDto);
        Task<bool> DeleteProductAsync(Guid id);

        // **Services**
        Task<(IEnumerable<ServiceSchema> ServicesSchema, int TotalCount)> GetAllServicesAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10);
        Task<ServiceSchema?> GetServiceByIdAsync(Guid id);
        Task<ServiceSchema> AddServiceAsync(ServiceDTO serviceDto);
        Task<Service?> UpdateServiceAsync(Guid serviceId, ServiceDTO updatedServiceDto);
        Task<bool> DeleteServiceAsync(Guid id);

        // **Categories**
        Task<(IEnumerable<ProductCategorySchema> Categories, int TotalCount)> GetAllCategoriesAsync(int skip = 0, int limit = 10);
        Task<ProductCategorySchema?> GetCategoryByIdAsync(Guid categoryId);
        Task<ProductCategorySchema> AddCategoryAsync(CategoryDTO categoryDto);
        Task<ProductCategorySchema?> UpdateCategoryAsync(Guid categoryId, CategoryDTO categoryDto);
        Task<bool> DeleteCategoryAsync(Guid categoryId);
    }
}
