using MediatR;
using PSPOS.ApiService.Services.Interfaces;

namespace PSPOS.ApiService.Events.Handlers
{
    public class SendSmsOnReservationModificationHandler : INotificationHandler<ReservationModifiedEvent>
    {
        private readonly ISmsService _smsService;

        public SendSmsOnReservationModificationHandler(ISmsService smsService)
        {
            _smsService = smsService;
        }

        public async Task Handle(ReservationModifiedEvent notification, CancellationToken cancellationToken)
        {
            var message = $"Your reservation (ID: {notification.ReservationId}) has been modified (status: {notification.Status}). The reservation date is {notification.AppointmentTime}.";
            await _smsService.SendSmsAsync(notification.CustomerPhone, message);
        }
    }
}