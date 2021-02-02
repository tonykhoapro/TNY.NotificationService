using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNY.NotificationService.BUS.Models;

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
        public bool? IsRoutine { get; set; }
        public Routine Routine { get; set; }
    }

    public class RoutineModel
    {
        public string Type { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public DateTime? Time { get; set; }
    }
}