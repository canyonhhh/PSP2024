using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using PSPOS.ApiService.Services.Interfaces;

namespace PSPOS.ApiService.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public TwilioSmsService(string accountSid, string authToken, string fromPhoneNumber)
        {
            _accountSid = accountSid;
            _authToken = authToken;
            _fromPhoneNumber = fromPhoneNumber;

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                var result = await MessageResource.CreateAsync(
                    to: new PhoneNumber(toPhoneNumber),
                    from: new PhoneNumber(_fromPhoneNumber),
                    body: message
                );

                // Check Twilio's response status
                return result.Status != MessageResource.StatusEnum.Failed &&
                       result.Status != MessageResource.StatusEnum.Undelivered;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send SMS: {ex.Message}");
                return false;
            }
        }
    }
}