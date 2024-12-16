namespace PSPOS.ServiceDefaults.Models
{
    public class Discount : BaseClass
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Method { get; set; }
        public bool Active { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public DateTime EndDate { get; set; }
        public Guid BusinessId { get; set; }
        public Guid? ProductOrServiceGroupId { get; set; }
    }
}