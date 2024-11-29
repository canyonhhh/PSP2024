using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync()
        {
            return await _discountRepository.GetAllDiscountsAsync();
        }

        public async Task<Discount?> GetDiscountByIdAsync(Guid discountId)
        {
            return await _discountRepository.GetDiscountByIdAsync(discountId);
        }

        public async Task AddDiscountAsync(Discount discount)
        {
            await _discountRepository.AddDiscountAsync(discount);
        }

        public async Task UpdateDiscountAsync(Guid discountId, Discount updatedDiscount)
        {
            await _discountRepository.UpdateDiscountAsync(discountId, updatedDiscount);
        }

        public async Task DeleteDiscountAsync(Guid discountId)
        {
            await _discountRepository.DeleteDiscountAsync(discountId);
        }

        public async Task ApplyDiscountToOrderItemAsync(Guid orderId, Guid orderItemId, Guid discountId)
        {
            await _discountRepository.ApplyDiscountToOrderItemAsync(orderId, orderItemId, discountId);
        }
    }
}
