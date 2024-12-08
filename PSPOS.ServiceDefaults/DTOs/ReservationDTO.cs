using Google.Protobuf.WellKnownTypes;
using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.Models;

public class ReservationDTO
{
    public ReservationDTO(string customerFirstName, string customerLastName, string? customerEmail, string? customerPhoneNumber, Timestamp appointmentTime, ReservationStatus status, Guid serviceId)
    {
        CustomerFirstName = customerFirstName;
        CustomerLastName = customerLastName;
        CustomerEmail = customerEmail;
        CustomerPhoneNumber = customerPhoneNumber;
        AppointmentTime = appointmentTime;
        Status = status;
        ServiceId = serviceId;
    }

    [Required(ErrorMessage = "CustomerFirstName is required")]
    [StringLength(50, ErrorMessage = "CustomerFirstName cannot be longer than 50 characters")]
    public string CustomerFirstName { get; set; }

    [Required(ErrorMessage = "CustomerLastName is required")]
    [StringLength(50, ErrorMessage = "CustomerLastName cannot be longer than 50 characters")]
    public string CustomerLastName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? CustomerEmail { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? CustomerPhoneNumber { get; set; }

    [Required(ErrorMessage = "AppointmentTime is required")]
    public Timestamp AppointmentTime { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(ReservationStatus), ErrorMessage = "Invalid status value")]
    public ReservationStatus Status { get; set; }

    [Required(ErrorMessage = "ServiceId is required")]
    public Guid ServiceId { get; set; }
}
