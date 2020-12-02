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
        public List<string> AppID { get; set; }
        public DateTime? ScheduleTime { get; set; }
        public bool? IsCancel { get; set; }
        public DateTime? SendTime { get; set; }
    }
}
