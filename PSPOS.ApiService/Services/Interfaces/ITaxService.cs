using PSPOS.ServiceDefaults.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSPOS.ApiService.Services.Interfaces
{
    public interface ITaxService
    {
        Task<IEnumerable<Tax>> GetAllTaxesAsync();
        Task<Tax?> GetTaxByIdAsync(Guid taxId);
        Task AddTaxAsync(Tax tax);
        Task UpdateTaxAsync(Guid taxId, Tax updatedTax);
        Task DeleteTaxAsync(Guid taxId);
    }
}