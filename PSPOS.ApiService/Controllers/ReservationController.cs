using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;
using Serilog;

namespace PSPOS.ApiService.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    [Produces("application/json")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Reservation>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations(
            [FromQuery] string? customer,
            [FromQuery] string? status,
            [FromQuery] Guid? serviceId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            Log.Information("Fetching reservations with filters - Customer: {Customer}, Status: {Status}, ServiceId: {ServiceId}, From: {From}, To: {To}", customer, status, serviceId, from, to);
            var reservations = await _reservationService.GetReservationsAsync(customer, status, serviceId, from, to);
            return Ok(reservations);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Reservation), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            Log.Information("Creating reservation for customer: {Customer}", reservation.CustomerEmail);
            if (!ModelState.IsValid)
            {
                Log.Warning("Invalid reservation model state for customer: {Customer}", reservation.CustomerEmail);
                return BadRequest(ModelState);
            }
            await _reservationService.AddReservationAsync(reservation);
            Log.Information("Reservation created successfully for customer: {Customer}", reservation.CustomerEmail);
            return CreatedAtAction(nameof(GetReservationById), new { reservationId = reservation.Id }, reservation);
        }

        [HttpGet("{reservationId}")]
        [ProducesResponseType(typeof(Reservation), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Reservation>> GetReservationById(Guid reservationId)
        {
            Log.Information("Fetching reservation with ID: {ReservationId}", reservationId);
            var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
            if (reservation == null)
            {
                Log.Warning("Reservation not found with ID: {ReservationId}", reservationId);
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpPut("{reservationId}")]
        [ProducesResponseType(typeof(Reservation), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateReservation(Guid reservationId, [FromBody] Reservation reservation)
        {
            Log.Information("Updating reservation with ID: {ReservationId}", reservationId);
            if (reservationId != reservation.Id)
            {
                Log.Warning("Reservation ID mismatch. Provided ID: {ProvidedId}, Reservation ID: {ReservationId}", reservationId, reservation.Id);
                return BadRequest("Reservation ID mismatch.");
            }

            var existing = await _reservationService.GetReservationByIdAsync(reservationId);
            if (existing == null)
            {
                Log.Warning("Reservation not found with ID: {ReservationId}", reservationId);
                return NotFound();
            }

            reservation.CreatedAt = existing.CreatedAt;
            reservation.CreatedBy = existing.CreatedBy;

            await _reservationService.UpdateReservationAsync(reservation);
            Log.Information("Reservation updated successfully with ID: {ReservationId}", reservationId);
            return NoContent();
        }

        [HttpDelete("{reservationId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteReservation(Guid reservationId)
        {
            Log.Information("Deleting reservation with ID: {ReservationId}", reservationId);
            var existing = await _reservationService.GetReservationByIdAsync(reservationId);
            if (existing == null)
            {
                Log.Warning("Reservation not found with ID: {ReservationId}", reservationId);
                return NotFound();
            }
            await _reservationService.DeleteReservationAsync(reservationId);
            Log.Information("Reservation deleted successfully with ID: {ReservationId}", reservationId);
            return NoContent();
        }

        [HttpGet("free")]
        [ProducesResponseType(typeof(PaginatedResult<AvailableTimeDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PaginatedResult<AvailableTimeDto>>> GetAvailableTimes(
            [FromQuery] Guid serviceId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            Log.Information("Fetching available times for ServiceId: {ServiceId}, From: {From}, To: {To}, Page: {Page}, PageSize: {PageSize}", serviceId, from, to, page, pageSize);
            var availableTimes = await _reservationService.GetAvailableTimesAsync(serviceId, from, to, page, pageSize);
            return Ok(availableTimes);
        }
    }
}