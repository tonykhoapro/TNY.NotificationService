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
    public class Bus_NotificationActivity
    {
        private readonly IMongoCollection<NotificationActivity> _objLst;


        public Bus_NotificationActivity()
        {
            DBConnectionHelper connectionHelper = new DBConnectionHelper();
            _objLst = connectionHelper.database.GetCollection<NotificationActivity>("NotificationActivity");
        }

        public List<NotificationActivity> GetAll() => _objLst.Find(x => true).ToList();

        public List<NotificationActivity> Get_ByTime(DateTime fromTime, DateTime toTime) => _objLst.Find<NotificationActivity>(x => x.SendTime >= fromTime && x.SendTime <= toTime).ToList();

        public NotificationActivity Get_ById(string id) => _objLst.Find<NotificationActivity>(x => x.Id == id).FirstOrDefault();

        public List<NotificationActivity> Get_ByUserID(string userid) => _objLst.Find<NotificationActivity>(x => x.UserID == userid).ToList();

        public List<NotificationActivity> Get_Cancel() => _objLst.Find<NotificationActivity>(x => x.IsCancel == true).ToList();

        public NotificationActivity Create(NotificationActivity appinfo)
        {
            _objLst.InsertOne(appinfo);
            return appinfo;
        }

        public void Remove(string id) => _objLst.DeleteOne(x => x.Id == id);
    }
}
