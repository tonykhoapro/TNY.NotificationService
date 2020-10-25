using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNY.Utils;
using MongoDB.Driver;
using TNY.BUS.Models;

namespace TNY.BUS.Bus
{
    public class Bus_AppInfo
    {
        private readonly IMongoCollection<AppInfo> _appInfo;


        public Bus_AppInfo()
        {
            DBConnectionHelper connectionHelper = new DBConnectionHelper();
            _appInfo = connectionHelper.database.GetCollection<AppInfo>("AppInfo");
        }

        List<AppInfo> GetAll() => _appInfo.Find(x => true).ToList();

        AppInfo Get_ById(string id) => _appInfo.Find<AppInfo>(x => x.Id == id).FirstOrDefault();

        public AppInfo Create(AppInfo appinfo)
        {
            _appInfo.InsertOne(appinfo);
            return appinfo;
        }

        public void Remove(string id) => _appInfo.DeleteOne(x => x.Id == id);
    }
}
