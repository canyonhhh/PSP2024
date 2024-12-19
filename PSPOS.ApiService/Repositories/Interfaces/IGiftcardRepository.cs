using PSPOS.ServiceDefaults.Models;
using System.Threading.Tasks;

namespace PSPOS.ApiService.Repositories.Interfaces
{
    public interface IGiftcardRepository
    {
        Task<IEnumerable<Giftcard>> GetAllGiftcardsAsync(DateTime? from, DateTime? to, int page, int pageSize);
        Task<Giftcard?> GetGiftcardByIdAsync(Guid giftcardId);
        Task AddGiftcardAsync(Giftcard giftcard);
        Task UpdateGiftcardAsync(Giftcard giftcard);
        Task DeleteGiftcardAsync(Guid giftcardId);
        Task<Giftcard?> GetGiftCardByCode(string giftcardCode);
    }
}
