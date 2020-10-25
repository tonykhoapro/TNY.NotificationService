using MongoDB.Driver;
using MongoDB.Bson;

namespace TNY.Utils
{
    public class DBConnectionHelper
    {
        public IMongoDatabase database;

        public DBConnectionHelper()
        {
            ConfigHelper config = new ConfigHelper();
            MongoClient client = new MongoClient(config.DBConnectionString);
            //MongoServer server = client.GetServer();
            database = client.GetDatabase(config.DBName);
        }
    }
}
