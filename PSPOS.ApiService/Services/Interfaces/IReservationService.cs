using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetReservationsAsync(
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
