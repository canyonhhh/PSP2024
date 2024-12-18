using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ServiceDefaults.Schemas
{
    public class ServiceSchema : BaseClass
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public decimal price { get; set; }
        public TimeSpan duration { get; set; }
        public Guid employeeId { get; set; }
    }
}