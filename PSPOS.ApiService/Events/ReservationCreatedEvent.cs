using MediatR;

namespace PSPOS.ApiService.Events
{
    public class ReservationCreatedEvent : INotification
    {
        public Guid ReservationId { get; }
        public string CustomerPhone { get; }
        public DateTime AppointmentTime { get; set; }

        public ReservationCreatedEvent(Guid reservationId, string customerPhone, DateTime appointmentTime)
        {
            ReservationId = reservationId;
            CustomerPhone = customerPhone;
            AppointmentTime = appointmentTime;
        }

    }
}