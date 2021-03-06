﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TNY.NotificationService.BUS.Models;
using TNY.NotificationService.Utils;

namespace TNY.NotificationService.BUS
{
    public class Bus_AppInfo
    {
        private readonly IMongoCollection<AppInfo> _appInfo;


        public Bus_AppInfo()
        {
            DBConnectionHelper connectionHelper = new DBConnectionHelper();
            _appInfo = connectionHelper.database.GetCollection<AppInfo>("AppInfo");
        }

        public List<AppInfo> GetAll() => _appInfo.Find(x => true).ToList();

        public AppInfo Get_ById(string id) => _appInfo.Find<AppInfo>(x => x.Id == id).FirstOrDefault();

        public AppInfo Create(AppInfo appinfo)
        {
            _appInfo.InsertOne(appinfo);
            return appinfo;
        }

        public bool Update(AppInfo appInfo)
        {
            ReplaceOneResult _rs = _appInfo.ReplaceOne(_ => _.Id == appInfo.Id, appInfo);
            return _rs.IsAcknowledged;
        }

        public void Remove(string id) => _appInfo.DeleteOne(x => x.Id == id);
    }
}
