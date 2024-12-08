namespace PSPOS.ServiceDefaults.Models;

public class Transaction : BaseClass
{
    public Transaction(TransactionType transactionType)
    {
        TransactionType = transactionType;
    }

    public TransactionType TransactionType { get; set; }
}
