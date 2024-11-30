using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;

        public BusinessService(IBusinessRepository businessRepository)
        {
            _businessRepository = businessRepository;
        }

        public async Task<(IEnumerable<Business> Businesses, int TotalCount)> GetBusinessesAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
        {
            return await _businessRepository.GetBusinessesAsync(from, to, page, pageSize);
        }

        public async Task<Business?> GetBusinessByIdAsync(Guid businessId)
        {
            return await _businessRepository.GetBusinessByIdAsync(businessId);
        }

        public async Task<Business> CreateBusinessAsync(Business business)
        {
            if (await _businessRepository.BusinessExistsAsync(business.Id))
            {
                throw new InvalidOperationException("Business with the given ID already exists.");
            }

            return await _businessRepository.CreateBusinessAsync(business);
        }

        public async Task<Business?> UpdateBusinessAsync(Guid businessId, Business updatedBusiness)
        {
            if (!await _businessRepository.BusinessExistsAsync(businessId))
            {
                return null;
            }

            return await _businessRepository.UpdateBusinessAsync(businessId, updatedBusiness);
        }

        public async Task<bool> DeleteBusinessAsync(Guid businessId)
        {
            if (!await _businessRepository.BusinessExistsAsync(businessId))
            {
                return false;
            }

            return await _businessRepository.DeleteBusinessAsync(businessId);
        }
    }
}
