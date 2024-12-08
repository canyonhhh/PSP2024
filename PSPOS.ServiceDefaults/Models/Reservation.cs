using Google.Protobuf.WellKnownTypes;

namespace PSPOS.ServiceDefaults.Models;

public class Reservation : BaseClass
{
    public Reservation(string customerFirstName, string customerLastName, string? customerEmail, string? customerPhoneNumber, Timestamp appointmentTime, ReservationStatus status, Guid serviceId)
    {
        CustomerFirstName = customerFirstName;
        CustomerLastName = customerLastName;
        CustomerEmail = customerEmail;
        CustomerPhoneNumber = customerPhoneNumber;
        AppointmentTime = appointmentTime;
        Status = status;
        ServiceId = serviceId;
    }
    public string CustomerFirstName { get; set; }
    public string CustomerLastName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhoneNumber { get; set; }
    public Timestamp AppointmentTime { get; set; }
    public ReservationStatus Status { get; set; }
    public Guid ServiceId { get; set; }
}