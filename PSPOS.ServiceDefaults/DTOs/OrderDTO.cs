﻿namespace PSPOS.ServiceDefaults.DTOs
{
    public class OrderDTO
    {
        public Guid businessId { get; set; }
        public string? status { get; set; }
        public string? currency { get; set; }
    }
}