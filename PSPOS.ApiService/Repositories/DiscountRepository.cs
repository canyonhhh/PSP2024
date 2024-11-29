using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext _context;

        public DiscountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync()
        {
            return await _context.Discounts.ToListAsync();
        }

        public async Task<Discount?> GetDiscountByIdAsync(Guid discountId)
        {
            return await _context.Discounts.FindAsync(discountId);
        }

        public async Task AddDiscountAsync(Discount discount)
        {
            await _context.Discounts.AddAsync(discount);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDiscountAsync(Guid discountId, Discount updatedDiscount)
        {
            var discount = await _context.Discounts.FindAsync(discountId);
            if (discount == null)
                throw new ArgumentException($"Discount with ID {discountId} does not exist.");

            discount.Name = updatedDiscount.Name;
            discount.Type = updatedDiscount.Type;
            discount.Value = updatedDiscount.Value;

            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDiscountAsync(Guid discountId)
        {
            var discount = await _context.Discounts.FindAsync(discountId);
            if (discount != null)
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ApplyDiscountToOrderItemAsync(Guid orderId, Guid orderItemId, Guid discountId)
        {
            var orderItem = await _context.OrderItems.FindAsync(orderItemId);
            if (orderItem == null)
                throw new ArgumentException($"OrderItem with ID {orderItemId} does not exist.");

            var discount = await _context.Discounts.FindAsync(discountId);
            if (discount == null)
                throw new ArgumentException($"Discount with ID {discountId} does not exist.");

            if (discount.Type == DiscountType.Percentage)
            {
                orderItem.Price -= orderItem.Price * (discount.Value / 100);
            }
            else
            {
                orderItem.Price -= discount.Value;
            }

            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();
        }
    }
}
