namespace PSPOS.ServiceDefaults.DTOs
{
    public class TransactionDTO
    {
        public IEnumerable<Guid>? itemIds { get; set; }
        public decimal paidByCash { get; set; }
        public decimal paidByGiftcard { get; set; }
        public decimal paidByBankcard { get; set; }
        public string? externalTransactionId { get; set; } // for stripe
        public Guid giftcardId { get; set; }
    }
}