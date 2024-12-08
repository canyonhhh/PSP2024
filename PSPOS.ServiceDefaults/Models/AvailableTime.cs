namespace PSPOS.ServiceDefaults.Models
{
    public class AvailableTime
    {
        public Guid ServiceId { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
    }
}
