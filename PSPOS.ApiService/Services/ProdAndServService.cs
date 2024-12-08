using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

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

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _repository.GetAllProductsAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _repository.GetProductByIdAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _repository.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _repository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _repository.DeleteProductAsync(id);
        }

        // **Services**

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _repository.GetAllServicesAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(Guid id)
        {
            return await _repository.GetServiceByIdAsync(id);
        }

        public async Task AddServiceAsync(Service service)
        {
            await _repository.AddServiceAsync(service);
        }

        public async Task UpdateServiceAsync(Service service)
        {
            await _repository.UpdateServiceAsync(service);
        }

        public async Task DeleteServiceAsync(Guid id)
        {
            await _repository.DeleteServiceAsync(id);
        }
    }
}
