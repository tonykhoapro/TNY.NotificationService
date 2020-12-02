using MongoDB.Driver;
using MongoDB.Bson;
using System;

namespace TNY.NotificationService.Utils
{
    public class DBConnectionHelper
    {
        public IMongoDatabase database;

        public DBConnectionHelper()
        {
            try
            {
                MongoClient client = new MongoClient(ConfigHelper.DBConnectionString);
                //MongoServer server = client.GetServer();
                database = client.GetDatabase(ConfigHelper.DBName);
            }
            catch(Exception ex)
            {
                LogGenerationHelper.WriteToFile(ex.Message);
            }
        }
    }
}
