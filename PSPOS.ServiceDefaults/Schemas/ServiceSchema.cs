using Microsoft.VisualBasic;
using PSPOS.ServiceDefaults.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSPOS.ServiceDefaults.Schemas
{
    public class ServiceSchema : BaseClass
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public decimal price { get; set; }
        public DateInterval interval { get; set; }
        public Guid employeeId { get; set; }
    }
}