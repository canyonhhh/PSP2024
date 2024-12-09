using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ServiceDefaults.Schemas
{
    public class OrderSchema : BaseClass
    {
        public Guid businessId { get; set; }
        public string? status { get; set; }
        public string? currency { get; set; }
    }
}