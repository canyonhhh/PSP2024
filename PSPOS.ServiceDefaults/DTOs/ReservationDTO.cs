namespace PSPOS.ServiceDefaults.DTOs
{
    public class ReservationDTO
    {
        public Guid Id { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime AppointmentTime { get; set; }
        public Guid ServiceId { get; set; }
        public ReservationStatus Status { get; set; }
    }
}