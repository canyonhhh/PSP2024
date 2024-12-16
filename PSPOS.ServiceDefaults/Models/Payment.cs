namespace PSPOS.ServiceDefaults.Models;

public class Payment : BaseClass
{
    public Payment(PaymentMethod method, decimal amount, Currency paymentCurrency, String externalPaymentId, Guid transactionId, Guid giftCardId)
    {
        Method = method;
        Amount = amount;
        PaymentCurrency = paymentCurrency;
        ExternalPaymentId = externalPaymentId;
        TransactionId = transactionId;
        GiftCardId = giftCardId;
    }

    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public Currency PaymentCurrency { get; set; }
    public String ExternalPaymentId { get; set; }
    public Guid TransactionId { get; set; }
    public Guid GiftCardId { get; private set; }
}