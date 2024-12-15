using PSPOS.ServiceDefaults.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSPOS.ServiceDefaults.Schemas
{
    public class ProductCategorySchema : BaseClass
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public Guid[]? productOrServiceIds { get; set; }
    }
}