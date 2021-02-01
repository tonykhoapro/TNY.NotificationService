using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNY.NotificationService.BUS.Models;
using TNY.NotificationService.Utils;

namespace TNY.NotificationService.BUS
{
    public class Bus_Recipients
    {
        private readonly IMongoCollection<Recipients> _objLst;

        public Bus_Recipients()
        {
            DBConnectionHelper connectionHelper = new DBConnectionHelper();
            _objLst = connectionHelper.database.GetCollection<Recipients>("Recipients");
        }

        public List<Recipients> GetAll() => _objLst.Find(x => true).ToList();

        public Recipients Get_ById(string id) => _objLst.Find<Recipients>(x => x.RecipientID == id).FirstOrDefault();

        public Recipients Get_ByIdAppID(string id, string appid) => _objLst.Find<Recipients>(x => x.RecipientID == id && x.AppID == appid && x.ReceivedTime == null).FirstOrDefault();

        public Recipients Create(Recipients recipient)
        {
            _objLst.InsertOne(recipient);
            return recipient;
        }

        public Recipients Replace(Recipients recipient)
        {
            var _filter = Builders<Recipients>.Filter.Eq(x=>x.RecipientID, recipient.RecipientID)
                & Builders<Recipients>.Filter.Eq(x => x.AppID, recipient.AppID)
                & Builders<Recipients>.Filter.Eq("ReceivedTime", BsonNull.Value);
            _objLst.ReplaceOne(_filter, recipient);
            return recipient;
        }

        public Recipients Update_ReceiveTime(Recipients recipient)
        {
            var _filter = Builders<Recipients>.Filter.Eq(x => x.RecipientID, recipient.RecipientID)
                & Builders<Recipients>.Filter.Eq(x => x.AppID, recipient.AppID)
                & Builders<Recipients>.Filter.Eq("ReceivedTime", BsonNull.Value);
            var _update = Builders<Recipients>.Update.Set(x => x.ReceivedTime, DateTime.Now);
            _objLst.UpdateOne(_filter, _update);
            return recipient;
        }

        public void Remove(string id) => _objLst.DeleteOne(x => x.RecipientID == id);
    }
}
