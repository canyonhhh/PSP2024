using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories.Interfaces;
public interface IBusinessRepository
{
    Task<(IEnumerable<Business>, int TotalCount)> GetBusinessesAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10);

    Task<Business?> GetBusinessByIdAsync(Guid businessId);

    Task<Business> CreateBusinessAsync(Business business);

    Task<Business?> UpdateBusinessAsync(Guid businessId, Business updatedBusiness);

    Task<bool> DeleteBusinessAsync(Guid businessId);

    Task<bool> BusinessExistsAsync(Guid businessId);
}