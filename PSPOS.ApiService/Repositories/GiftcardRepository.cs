using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories
{
    public class GiftcardRepository : IGiftcardRepository
    {
        private readonly AppDbContext _context;

        public GiftcardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Giftcard>> GetAllGiftcardsAsync(DateTime? from, DateTime? to, int page, int pageSize)
        {
            var query = _context.GiftCards.AsQueryable();

            if (from.HasValue)
                query = query.Where(d => d.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(d => d.CreatedAt <= to.Value);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Giftcard?> GetGiftcardByIdAsync(Guid giftcardId)
        {
            return await _context.GiftCards.FindAsync(giftcardId);
        }

        public async Task AddGiftcardAsync(Giftcard giftcard)
        {
            await _context.GiftCards.AddAsync(giftcard);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGiftcardAsync(Giftcard giftcard)
        {
            _context.GiftCards.Update(giftcard);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGiftcardAsync(Guid giftcardId)
        {
            var giftcard = await _context.GiftCards.FindAsync(giftcardId);
            if (giftcard != null)
            {
                _context.GiftCards.Remove(giftcard);
                await _context.SaveChangesAsync();
            }
        }
    }
}
