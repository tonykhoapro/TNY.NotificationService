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
    public class Bus_User
    {
        private readonly IMongoCollection<User> _objLst;

        public Bus_User()
        {
            DBConnectionHelper connectionHelper = new DBConnectionHelper();
            _objLst = connectionHelper.database.GetCollection<User>("User");
        }

        public List<User> GetAll() => _objLst.Find(x => true).ToList();

        public User Get_ById(string id) => _objLst.Find<User>(x => x.Id == id).FirstOrDefault();

        public User Get_ByLogin(string userName, string pwd) => _objLst.Find<User>(x => x.UserName == userName && x.Password == pwd).FirstOrDefault();

        public User Create(User user)
        {
            _objLst.InsertOne(user);
            return user;
        }

        public bool Update(User model)
        {
            return _objLst.ReplaceOne(_ => _.Id == model.Id, model).IsAcknowledged;
        }

        public void Remove(string id) => _objLst.DeleteOne(x => x.Id == id);
    }
}
