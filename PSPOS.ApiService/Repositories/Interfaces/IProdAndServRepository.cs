using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories.Interfaces;
public interface IProdAndServRepository
{
    // **Products**
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(Guid id);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Guid id);

    // **Services**
    Task<IEnumerable<Service>> GetAllServicesAsync();
    Task<Service?> GetServiceByIdAsync(Guid id);
    Task AddServiceAsync(Service service);
    Task UpdateServiceAsync(Service service);
    Task DeleteServiceAsync(Guid id);
}
