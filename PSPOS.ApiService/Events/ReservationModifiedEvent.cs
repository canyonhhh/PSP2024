using MediatR;

namespace PSPOS.ApiService.Events
{
    public class ReservationModifiedEvent : INotification
    {
        public Guid ReservationId { get; }
        public string CustomerPhone { get; }
        public DateTime AppointmentTime { get; set; }
        public ReservationStatus Status { get; set; }

        public ReservationModifiedEvent(Guid reservationId, string customerPhone, DateTime appointmentTime, ReservationStatus status)
        {
            ReservationId = reservationId;
            CustomerPhone = customerPhone;
            AppointmentTime = appointmentTime;
            Status = status;
        }

    }
}