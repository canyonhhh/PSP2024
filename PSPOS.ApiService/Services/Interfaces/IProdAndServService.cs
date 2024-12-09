using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services.Interfaces
{
    public interface IProdAndServService
    {
        // **Products**
        Task<(IEnumerable<Product> Products, int TotalCount)> GetAllProductsAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10);
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<Product> AddProductAsync(Product product);
        Task<Product?> UpdateProductAsync(Guid productId, Product updatedProduct);
        Task<bool> DeleteProductAsync(Guid id);

        // **Services**
        Task<(IEnumerable<Service> Services, int TotalCount)> GetAllServicesAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10);
        Task<Service?> GetServiceByIdAsync(Guid id);
        Task<Service> AddServiceAsync(Service service);
        Task<Service?> UpdateServiceAsync(Guid serviceId, Service updatedService);
        Task<bool> DeleteServiceAsync(Guid id);
    }
}
