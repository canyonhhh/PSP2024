namespace PSPOS.ServiceDefaults.Models
{
    public class Tax : BaseClass
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Active { get; set; }
        public float Percentage { get; set; }
        public Guid BusinessId { get; set; }
        public Guid? ProductOrServiceGroupId { get; set; }
        public Guid? ProductOrServiceId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}