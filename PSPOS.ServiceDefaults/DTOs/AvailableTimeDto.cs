namespace PSPOS.ServiceDefaults.DTOs
{
    public class AvailableTimeDto
    {
        public Guid ServiceId { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
    }
}