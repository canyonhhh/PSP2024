﻿using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ServiceDefaults.DTOs
{
    public class OrderItemDTO
    {
        public string? type { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public Guid orderId { get; set; }
        public Guid serviceId { get; set; }
        public Guid productId { get; set; }
        public Guid transactionId { get; set; }
    }
}