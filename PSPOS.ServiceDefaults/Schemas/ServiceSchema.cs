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
        public Guid id { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public decimal price { get; set; }
        public DateInterval interval { get; set; }
        public Guid employeeId { get; set; }
    }
}