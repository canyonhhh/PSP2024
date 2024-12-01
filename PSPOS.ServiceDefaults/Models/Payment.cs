namespace PSPOS.ServiceDefaults.Models;

public class Payment : BaseClass
{
    public Payment(PaymentMethod method, decimal amount, Currency paymentCurrency, Guid externalPaymentId, Guid transactionId, Guid giftcardId)
    {
        Method = method;
        Amount = amount;
        PaymentCurrency = paymentCurrency;
        ExternalPaymentId = externalPaymentId;
        TransactionId = transactionId;
        GiftcardId = giftcardId;
    }

    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public Currency PaymentCurrency { get; set; }
    public Guid ExternalPaymentId { get; set; }
    public Guid TransactionId { get; set; }
    public Guid GiftcardId { get; private set; }
}
