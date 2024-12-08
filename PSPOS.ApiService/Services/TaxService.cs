using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSPOS.ApiService.Services
{
    public class TaxService : ITaxService
    {
        private readonly ITaxRepository _taxRepository;

        public TaxService(ITaxRepository taxRepository)
        {
            _taxRepository = taxRepository;
        }

        public async Task<IEnumerable<Tax>> GetAllTaxesAsync()
        {
            return await _taxRepository.GetAllTaxesAsync();
        }

        public async Task<Tax?> GetTaxByIdAsync(Guid taxId)
        {
            return await _taxRepository.GetTaxByIdAsync(taxId);
        }

        public async Task AddTaxAsync(Tax tax)
        {
            await _taxRepository.AddTaxAsync(tax);
        }

        public async Task UpdateTaxAsync(Guid taxId, Tax updatedTax)
        {
            var existingTax = await _taxRepository.GetTaxByIdAsync(taxId);
            if (existingTax == null)
                throw new KeyNotFoundException("Tax not found");

            existingTax.Name = updatedTax.Name;
            existingTax.Description = updatedTax.Description;
            existingTax.Active = updatedTax.Active;
            existingTax.Percentage = updatedTax.Percentage;
            existingTax.BusinessId = updatedTax.BusinessId;
            existingTax.ProductOrServiceGroupId = updatedTax.ProductOrServiceGroupId;
            existingTax.ProductOrServiceId = updatedTax.ProductOrServiceId;
            existingTax.UpdatedAt = DateTime.UtcNow;

            await _taxRepository.UpdateTaxAsync(existingTax);
        }


        public async Task DeleteTaxAsync(Guid taxId)
        {
            await _taxRepository.DeleteTaxAsync(taxId);
        }
    }
}