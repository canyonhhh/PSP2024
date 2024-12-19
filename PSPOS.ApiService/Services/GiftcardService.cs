using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services
{
    public class GiftcardService : IGiftcardService
    {
        private readonly IGiftcardRepository _giftcardRepository;

        public GiftcardService(IGiftcardRepository giftcardRepository)
        {
            _giftcardRepository = giftcardRepository;
        }

        public async Task<IEnumerable<Giftcard>> GetAllGiftcardsAsync(DateTime? from, DateTime? to, int page, int pageSize)
        {
            return await _giftcardRepository.GetAllGiftcardsAsync(from, to, page, pageSize);
        }

        public async Task<Giftcard?> GetGiftcardByIdAsync(Guid giftcardId)
        {
            return await _giftcardRepository.GetGiftcardByIdAsync(giftcardId);
        }

        public async Task AddGiftcardAsync(Giftcard giftcard)
        {
            await _giftcardRepository.AddGiftcardAsync(giftcard);
        }

        public async Task UpdateGiftcardAsync(Guid giftcardId, Giftcard updatedGiftcard)
        {
            var existingGiftcard = await _giftcardRepository.GetGiftcardByIdAsync(giftcardId);
            if (existingGiftcard == null)
                throw new KeyNotFoundException("Giftcard not found");


            existingGiftcard.Id = updatedGiftcard.Id;
            existingGiftcard.Amount = updatedGiftcard.Amount;
            existingGiftcard.Code = updatedGiftcard.Code;
            existingGiftcard.BusinessId = updatedGiftcard.BusinessId;

            await _giftcardRepository.UpdateGiftcardAsync(existingGiftcard);
        }

        public async Task DeleteGiftcardAsync(Guid giftcardId)
        {
            await _giftcardRepository.DeleteGiftcardAsync(giftcardId);
        }
    }
}