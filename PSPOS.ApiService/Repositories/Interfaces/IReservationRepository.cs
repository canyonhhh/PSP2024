using PSPOS.ServiceDefaults.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSPOS.ApiService.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetFilteredReservationsAsync(
            string? customer,
            string? status,
            Guid? serviceId,
            DateTime? from,
            DateTime? to);
        Task<Reservation?> GetReservationByIdAsync(Guid id);
        Task AddReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(Guid id);
        Task<PaginatedResult<AvailableTime>> GetAvailableTimesAsync(
            Guid serviceId,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize);
    }
}
