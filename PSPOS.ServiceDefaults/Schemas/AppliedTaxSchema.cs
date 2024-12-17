using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSPOS.ServiceDefaults.Schemas
{
    public class AppliedTaxSchema
    {
        public Guid Id { get; set; }
        public decimal percentage { get; set; }
        public Guid taxId { get; set; }
        public Guid orderItemId { get; set; }
        public Guid orderId { get; set; }
    }
}
