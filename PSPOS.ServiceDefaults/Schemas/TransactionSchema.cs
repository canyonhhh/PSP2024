namespace PSPOS.ServiceDefaults.Schemas
{
    public class TransactionSchema
    {
        public Guid id { get; set; }
        public string? status { get; set; }
        public decimal paidByCash { get; set; }
        public decimal paidByGiftcard { get; set; }
        public decimal paidByBankcard { get; set; }
        public Guid externalId { get; set; }
    }
}