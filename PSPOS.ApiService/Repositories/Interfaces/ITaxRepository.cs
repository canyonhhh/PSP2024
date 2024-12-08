using PSPOS.ServiceDefaults.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSPOS.ApiService.Repositories.Interfaces
{
    public interface ITaxRepository
    {
        Task<IEnumerable<Tax>> GetAllTaxesAsync();
        Task<Tax?> GetTaxByIdAsync(Guid taxId);
        Task AddTaxAsync(Tax tax);
        Task UpdateTaxAsync(Tax tax);
        Task DeleteTaxAsync(Guid taxId);
    }
}