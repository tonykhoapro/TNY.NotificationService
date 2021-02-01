using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNY.NotificationService.BUS.Models
{
    public class Recipients
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string RecipientID { get; set; }
        public string AppID { get; set; }
        public List<string> NotifIDs { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ReceivedTime { get; set; }
    }
}
