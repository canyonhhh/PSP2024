using PSPOS.ServiceDefaults.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSPOS.ApiService.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<Discount>> GetAllDiscountsAsync(DateTime? from, DateTime? to, int page, int pageSize);
        Task<Discount?> GetDiscountByIdAsync(Guid discountId);
        Task AddDiscountAsync(Discount discount);
        Task AddAppliedDiscountAsync(AppliedDiscount discount);
        Task UpdateDiscountAsync(Discount discount);
        Task DeleteDiscountAsync(Guid discountId);
    }
}