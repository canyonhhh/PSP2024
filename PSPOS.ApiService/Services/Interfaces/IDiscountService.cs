﻿namespace PSPOS.ApiService.Services.Interfaces
{
    using PSPOS.ServiceDefaults.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDiscountService
    {
        Task<IEnumerable<Discount>> GetAllDiscountsAsync();
        Task<Discount?> GetDiscountByIdAsync(Guid discountId);
        Task AddDiscountAsync(Discount discount);
        Task UpdateDiscountAsync(Guid discountId, Discount updatedDiscount);
        Task DeleteDiscountAsync(Guid discountId);
        Task ApplyDiscountToOrderItemAsync(Guid orderId, Guid orderItemId, Guid discountId);
    }
}
