using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;
        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetFilteredReservationsAsync(
            string? customer,
            string? status,
            Guid? serviceId,
            DateTime? from,
            DateTime? to)
        {
            var query = _context.Reservations.AsQueryable();

            if (!string.IsNullOrEmpty(customer))
            {
                query = query.Where(r => (r.CustomerFirstName + " " + r.CustomerLastName).Contains(customer));
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<ReservationStatus>(status, true, out var parsedStatus))
                {
                    query = query.Where(r => r.Status == parsedStatus);
                }
            }

            if (serviceId.HasValue)
            {
                query = query.Where(r => r.ServiceId == serviceId.Value);
            }

            if (from.HasValue)
            {
                query = query.Where(r => r.AppointmentTime >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(r => r.AppointmentTime <= to.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(Guid id)
        {
            return await _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }


        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            var trackedEntity = _context.Reservations.Local
                .FirstOrDefault(r => r.Id == reservation.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.Entry(reservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        public async Task DeleteReservationAsync(Guid id)
        {
            var reservation = await GetReservationByIdAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PaginatedResult<AvailableTimeDto>> GetAvailableTimesAsync(
            Guid serviceId,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
                throw new ArgumentException($"Service with ID {serviceId} not found.");

            var duration = service.Duration;
            var startDate = from ?? DateTime.Today;
            var endDate = to ?? startDate.AddDays(7);
            var existingReservations = await _context.Reservations
                .Where(r => r.ServiceId == serviceId && r.AppointmentTime >= startDate && r.AppointmentTime <= endDate)
                .ToListAsync();

            var availableTimes = new List<AvailableTimeDto>();
            var businessDayStart = new TimeSpan(9, 0, 0);
            var businessDayEnd = new TimeSpan(17, 0, 0);

            for (var day = startDate.Date; day <= endDate.Date; day = day.AddDays(1))
            {
                var currentTime = day + businessDayStart;
                var endTime = day + businessDayEnd;

                while (currentTime.Add(duration) <= endTime)
                {
                    var conflicts = existingReservations.Any(r =>
                        currentTime < r.AppointmentTime.Add(duration) &&
                        currentTime.Add(duration) > r.AppointmentTime
                    );

                    if (!conflicts)
                    {
                        availableTimes.Add(new AvailableTimeDto
                        {
                            ServiceId = serviceId,
                            TimeFrom = currentTime,
                            TimeTo = currentTime.Add(duration)
                        });
                    }

                    currentTime = currentTime.AddMinutes(15);
                }
            }

            availableTimes = availableTimes.OrderBy(at => at.TimeFrom).ToList();
            var totalItems = availableTimes.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedItems = availableTimes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<AvailableTimeDto>
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                Items = paginatedItems
            };
        }
    }
}