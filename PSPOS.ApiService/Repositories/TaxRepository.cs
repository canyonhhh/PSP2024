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
    public class TaxRepository : ITaxRepository
    {
        private readonly AppDbContext _context;

        public TaxRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tax>> GetAllTaxesAsync()
        {
            return await _context.Taxes.ToListAsync();
        }

        public async Task<Tax?> GetTaxByIdAsync(Guid taxId)
        {
            return await _context.Taxes.FirstOrDefaultAsync(t => t.Id == taxId);
        }


        public async Task AddTaxAsync(Tax tax)
        {
            await _context.Taxes.AddAsync(tax);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateTaxAsync(Tax tax)
        {
            _context.Taxes.Update(tax);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaxAsync(Guid taxId)
        {
            var tax = await GetTaxByIdAsync(taxId);
            if (tax != null)
            {
                _context.Taxes.Remove(tax);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Tax entry not found.");
            }
        }

    }
}