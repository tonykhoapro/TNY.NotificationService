using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNY.NotificationService.WebAPI.Models
{
    public class NotificationModel
    {
        //public string UserID { get; set; }
        public string Id { get; set; }
        public string Content { get; set; }
        public List<string> RecipientIDs { get; set; }
        public DateTime ScheduleTime { get; set; }
        public List<string> AppIDs { get; set; }
    }
}