namespace PSPOS.ApiService.Services.Interfaces
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string toPhoneNumber, string message);
    }
}