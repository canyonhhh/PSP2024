using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;

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
            var reservations = await _reservationService.GetReservationsAsync(customer, status, serviceId, from, to);
            return Ok(reservations);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Reservation), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _reservationService.AddReservationAsync(reservation);
            return CreatedAtAction(nameof(GetReservationById), new { reservationId = reservation.Id }, reservation);
        }

        [HttpGet("{reservationId}")]
        [ProducesResponseType(typeof(Reservation), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Reservation>> GetReservationById(Guid reservationId)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        [HttpPut("{reservationId}")]
        [ProducesResponseType(typeof(Reservation), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateReservation(Guid reservationId, [FromBody] Reservation reservation)
        {
            if (reservationId != reservation.Id)
                return BadRequest("Reservation ID mismatch.");

            var existing = await _reservationService.GetReservationByIdAsync(reservationId);
            if (existing == null)
                return NotFound();

            reservation.CreatedAt = existing.CreatedAt;
            reservation.CreatedBy = existing.CreatedBy;

            await _reservationService.UpdateReservationAsync(reservation);
            return NoContent();
        }


        [HttpDelete("{reservationId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteReservation(Guid reservationId)
        {
            var existing = await _reservationService.GetReservationByIdAsync(reservationId);
            if (existing == null) return NotFound();
            await _reservationService.DeleteReservationAsync(reservationId);
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
            var availableTimes = await _reservationService.GetAvailableTimesAsync(serviceId, from, to, page, pageSize);
            return Ok(availableTimes);
        }
    }
}