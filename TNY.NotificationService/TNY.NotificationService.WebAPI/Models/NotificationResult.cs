using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNY.NotificationService.WebAPI.Models
{
    public class NotificationResult
    {
        public string DeliveryId { get; set; }
        public long DeliveryQuantity { get; set; }
        public DateTime DeliveryTime { get; set; }
    }
}