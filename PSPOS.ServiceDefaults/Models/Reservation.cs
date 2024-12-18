using PSPOS.ServiceDefaults.Models;

public class Reservation : BaseClass
{
    public string? CustomerFirstName { get; set; }
    public string? CustomerLastName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime AppointmentTime { get; set; }
    public ReservationStatus Status { get; set; }
    public Guid ServiceId { get; set; }
    public Guid OrderId { get; set; }
}