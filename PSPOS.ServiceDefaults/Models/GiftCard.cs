namespace PSPOS.ServiceDefaults.Models;

public class Giftcard : BaseClass
{
    public Giftcard(decimal amount, string code, Guid businessId)
    {
        Amount = amount;
        Code = code;
        BusinessId = businessId;
    }

    public decimal Amount { get; set; }
    public string Code { get; set; }
    public Guid BusinessId { get; set; }
}