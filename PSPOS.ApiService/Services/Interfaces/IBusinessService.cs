using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services.Interfaces
{
    public interface IBusinessService
    {
        Task<(IEnumerable<Business> Businesses, int TotalCount)> GetBusinessesAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10);
        Task<Business?> GetBusinessByIdAsync(Guid businessId);
        Task<Business> CreateBusinessAsync(Business business);
        Task<Business?> UpdateBusinessAsync(Guid businessId, Business updatedBusiness);
        Task<bool> DeleteBusinessAsync(Guid businessId);
    }
}