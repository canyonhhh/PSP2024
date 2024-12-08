namespace PSPOS.ServiceDefaults.Models;

public class Discount : BaseClass
{
    public Discount(string name, DiscountMethod method, bool active, decimal? amount, decimal? percentage, DateTime endDate, Guid businessId)
    {
        Name = name;
        Method = method;
        Active = active;
        Amount = amount;
        Percentage = percentage;
        EndDate = endDate;
        BusinessId = businessId;
    }

    public string Name { get; set; }
    public DiscountMethod Method { get; set; }
    public bool Active { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Percentage { get; set; }
    public DateTime EndDate { get; set; }
    public Guid BusinessId { get; set; }
}