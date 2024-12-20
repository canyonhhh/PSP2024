using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.DTOs
{
    public class ReservationDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Customer first name is required.")]
        [MaxLength(50, ErrorMessage = "Customer first name cannot exceed 50 characters.")]
        public string? CustomerFirstName { get; set; }

        [Required(ErrorMessage = "Customer last name is required.")]
        [MaxLength(50, ErrorMessage = "Customer last name cannot exceed 50 characters.")]
        public string? CustomerLastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? CustomerEmail { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? CustomerPhone { get; set; }

        [Required(ErrorMessage = "Appointment time is required.")]
        public DateTime AppointmentTime { get; set; }

        [Required(ErrorMessage = "Service ID is required.")]
        public Guid ServiceId { get; set; }

        [Required(ErrorMessage = "Reservation status is required.")]
        public ReservationStatus Status { get; set; }

        [Required(ErrorMessage = "Order ID is required.")]
        public Guid OrderId { get; set; }
    }
}