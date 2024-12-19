using MediatR;
using PSPOS.ApiService.Services.Interfaces;

namespace PSPOS.ApiService.Events.Handlers
{
    public class SendSmsOnReservationCreatedHandler : INotificationHandler<ReservationCreatedEvent>
    {
        private readonly ISmsService _smsService;

        public SendSmsOnReservationCreatedHandler(ISmsService smsService)
        {
            _smsService = smsService;
        }

        public async Task Handle(ReservationCreatedEvent notification, CancellationToken cancellationToken)
        {
            var message = $"Your reservation (ID: {notification.ReservationId}) has been successfully created. The reservation date is {notification.AppointmentTime}.";
            await _smsService.SendSmsAsync(notification.CustomerPhone, message);
        }
    }
}