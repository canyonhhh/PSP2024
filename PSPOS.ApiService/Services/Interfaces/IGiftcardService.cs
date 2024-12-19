using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services.Interfaces
{
    public interface IGiftcardService
    {
        Task<IEnumerable<Giftcard>> GetAllGiftcardsAsync(DateTime? from, DateTime? to, int page, int pageSize);
        Task<Giftcard?> GetGiftcardByIdAsync(Guid giftcardId);
        Task AddGiftcardAsync(Giftcard giftcard);
        Task UpdateGiftcardAsync(Guid giftcardId, Giftcard updatedGiftcard);
        Task DeleteGiftcardAsync(Guid giftcardId);
    }
}