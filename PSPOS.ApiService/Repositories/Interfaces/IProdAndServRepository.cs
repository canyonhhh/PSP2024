using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories.Interfaces;
public interface IProdAndServRepository
{
    // **Products**
    Task<(IEnumerable<Product>, int totalCount)> GetAllProductsAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Product> AddProductAsync(Product product);
    Task<Product?> UpdateProductAsync(Guid productId, Product updatedProduct);
    Task<bool> DeleteProductAsync(Guid id);

    // **Services**
    Task<(IEnumerable<Service>, int totalCount)> GetAllServicesAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10);
    Task<Service?> GetServiceByIdAsync(Guid id);
    Task<Service> AddServiceAsync(Service service);
    Task<Service?> UpdateServiceAsync(Guid serviceId, Service updatedService);
    Task<bool> DeleteServiceAsync(Guid id);

    // **Product Stock**
    Task<ProductStock?> GetProductStockAsync(Guid productId);
    Task AddProductStockAsync(ProductStock productStock);
    Task<ProductStock?> UpdateProductStockAsync(ProductStock productStock);
    Task<bool> DeleteProductStockAsync(ProductStock productStock);
}
