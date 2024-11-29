namespace PSPOS.ServiceDefaults.Models
{
    public class Discount : BaseClass
    {
        public Discount(string name, DiscountType type, decimal value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public string Name { get; set; } // Name of the discount
        public DiscountType Type { get; set; } // Percentage or FixedAmount
        public decimal Value { get; set; } // Discount value
    }
}
