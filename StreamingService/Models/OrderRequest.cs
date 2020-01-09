using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingService.Models
{
    public class OrderRequest
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public int Quantity { get; set; }

        public OrderStatus status { get; set; }
    }
    public enum OrderStatus
    {
        IN_PROGRESS,
        COMPLETED,
        REJECTED
    }
}
