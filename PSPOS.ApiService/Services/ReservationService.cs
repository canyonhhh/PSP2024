using MediatR;
using PSPOS.ApiService.Events;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IMediator _mediator;

        public ReservationService(IReservationRepository reservationRepository, IMediator mediator)
        {
            _reservationRepository = reservationRepository;
            _mediator = mediator;
        }

        public async Task<IEnumerable<Reservation>> GetReservationsAsync(
            string? customer,
            string? status,
            Guid? serviceId,
            DateTime? from,
            DateTime? to)
        {
            return await _reservationRepository.GetFilteredReservationsAsync(customer, status, serviceId, from, to);
        }

        public async Task<Reservation?> GetReservationByIdAsync(Guid id)
        {
            return await _reservationRepository.GetReservationByIdAsync(id);
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            await _reservationRepository.AddReservationAsync(reservation);
            if (reservation.CustomerPhone != null)
            {
                var reservationEvent = new ReservationCreatedEvent(reservation.Id, reservation.CustomerPhone, reservation.AppointmentTime);
                await _mediator.Publish(reservationEvent);
            }

        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            await _reservationRepository.UpdateReservationAsync(reservation);
            if (reservation.CustomerPhone != null)
            {
                var reservationEvent = new ReservationModifiedEvent(reservation.Id, reservation.CustomerPhone, reservation.AppointmentTime, reservation.Status);
                await _mediator.Publish(reservationEvent);
            }
        }

        public async Task DeleteReservationAsync(Guid id)
        {
            await _reservationRepository.DeleteReservationAsync(id);
        }

        public async Task<PaginatedResult<AvailableTimeDto>> GetAvailableTimesAsync(
            Guid serviceId,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize)
        {
            return await _reservationRepository.GetAvailableTimesAsync(serviceId, from, to, page, pageSize);
        }
    }
}