using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNY.NotificationService.BUS.Models;
using TNY.NotificationService.Utils;
using MongoDB.Driver;

namespace TNY.NotificationService.BUS
{
    public class Bus_Templates
    {
        private readonly IMongoCollection<Templates> _objLst;

        public Bus_Templates()
        {
            DBConnectionHelper connectionHelper = new DBConnectionHelper();
            _objLst = connectionHelper.database.GetCollection<Templates>("Templates");
        }

        public List<Templates> GetAll() => _objLst.Find(x => true).ToList();

        public Templates Get_ById(string Id) => _objLst.Find<Templates>(x => x.Id == Id).FirstOrDefault();

        public Templates Get_ByType(string Type) => _objLst.Find<Templates>(x => x.Type == Type).FirstOrDefault();

        public Templates Create(Templates templates)
        {
            _objLst.InsertOne(templates);
            return templates;
        }

        public bool Update(Templates templates)
        {
            var _update = Builders<Templates>.Update.Set(x => x.Type, templates.Type)
            .Set(x => x.Name, templates.Name)
            .Set(x => x.Content, templates.Content)
            .Set(x => x.Type, templates.Type)
            .Set(x => x.UpdateTime, DateTime.Now);
            return _objLst.UpdateOne(_ => _.Id == templates.Id, _update).IsAcknowledged;
        }

        public void Remove(string Id) => _objLst.DeleteOne(x => x.Id == Id);
    }
}
