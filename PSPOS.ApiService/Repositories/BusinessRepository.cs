using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly AppDbContext _context;

        public BusinessRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Business>> GetBusinessesAsync()
        {
            return await _context.Businesses.ToListAsync();
        }

        public async Task<Business?> GetBusinessByIdAsync(Guid businessId)
        {
            return await _context.Businesses.FindAsync(businessId);
        }

        public async Task<Business> CreateBusinessAsync(Business business)
        {
            await _context.Businesses.AddAsync(business);
            await _context.SaveChangesAsync();
            return business;
        }

        public async Task<Business?> UpdateBusinessAsync(Guid businessId, Business updatedBusiness)
        {
            var existingBusiness = await GetBusinessByIdAsync(businessId);

            if (existingBusiness == null)
            {
                return null;
            }

            existingBusiness.Name = updatedBusiness.Name;
            existingBusiness.Email = updatedBusiness.Email;
            existingBusiness.Phone = updatedBusiness.Phone;
            existingBusiness.DefaultCurrency = updatedBusiness.DefaultCurrency;
            existingBusiness.AddressId = updatedBusiness.AddressId;

            _context.Businesses.Update(existingBusiness);
            await _context.SaveChangesAsync();

            return existingBusiness;
        }

        public async Task<bool> DeleteBusinessAsync(Guid businessId)
        {
            var business = await GetBusinessByIdAsync(businessId);

            if (business == null)
            {
                return false;
            }

            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> BusinessExistsAsync(Guid businessId)
        {
            return await _context.Businesses.AnyAsync(b => b.Id == businessId);
        }
    }
}