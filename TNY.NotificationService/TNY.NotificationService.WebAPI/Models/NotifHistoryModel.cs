using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNY.NotificationService.WebAPI.Models
{
    public class NotifHistoryModel
    {
        public string Id { get; set; }
        public string UserID { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
}