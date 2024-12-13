using PSPOS.ServiceDefaults.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PSPOS.ServiceDefaults.Schemas
{
    public class ProductSchema : BaseClass
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public decimal? price { get; set; }
        public string? imageUrl { get; set; }
        public int stockQuantity { get; set; }
        public Guid businessId { get; set; }
        public Guid? baseProductId { get; set; }
    }
}