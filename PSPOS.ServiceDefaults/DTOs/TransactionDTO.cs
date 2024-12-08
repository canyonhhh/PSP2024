namespace PSPOS.ServiceDefaults.DTOs
{
    public class TransactionDTO
    {
        public IEnumerable<Guid>? itemIds { get; set; }
        public decimal paidByCash { get; set; }
        public decimal paidByGiftcard { get; set; }
        public Guid giftcardId { get; set; }
    }
}
