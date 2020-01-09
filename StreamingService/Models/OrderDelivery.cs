using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingService.Models
{
    public class OrderDelivery
    {
        public int OrderRequestId {get; set;}
        public string CurrentLocation { get; set; }
        public string DeliveredBy { get; set; }
        
        public enum DeliveryStatus
        {
            IN_PROGRESS,
            DELIVERED,
            ON_HOLD,
            RETURNED
        }
    }
}
