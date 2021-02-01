using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNY.NotificationService.BUS.Models
{
    public class NotificationActivity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Content { get; set; }
        public string UserID { get; set; }
        public List<string> RecipientIDs { get; set; }
        public List<string> AppIDs { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ScheduleTime { get; set; }
        public bool? IsCancel { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? SendTime { get; set; }
        public bool? IsRoutine { get; set; }
        public Routine Routine { get; set; }
        //[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        //public DateTime? LastSentRoutine { get; set; }
    }

    public class Routine
    {
        public string Type { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? Time { get; set; }
    }

    public class RoutineType
    {
        public const string DAILY = "daily";
        public const string WEEKLY = "weekly";
        public const string MONTHLY = "monthly";
    }
}
