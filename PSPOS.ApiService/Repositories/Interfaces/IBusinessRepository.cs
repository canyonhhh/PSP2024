using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories.Interfaces;
public interface IBusinessRepository
{
    Task<IEnumerable<Business>> GetBusinessesAsync();

    Task<Business?> GetBusinessByIdAsync(Guid businessId);

    Task<Business> CreateBusinessAsync(Business business);

    Task<Business?> UpdateBusinessAsync(Guid businessId, Business updatedBusiness);

    Task<bool> DeleteBusinessAsync(Guid businessId);

    Task<bool> BusinessExistsAsync(Guid businessId);
}