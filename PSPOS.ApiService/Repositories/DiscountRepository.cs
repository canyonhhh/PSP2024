using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSPOS.ApiService.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext _context;

        public DiscountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync(DateTime? from, DateTime? to, int page, int pageSize)
        {
            var query = _context.Discounts.AsQueryable();

            if (from.HasValue)
                query = query.Where(d => d.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(d => d.CreatedAt <= to.Value);

            return await query
                .OrderBy(d => d.EndDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
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

        public async Task UpdateDiscountAsync(Discount discount)
        {
            discount.UpdatedAt = DateTime.Now; // Update timestamp
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
    }
}
