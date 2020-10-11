using MongoDB.Driver;

namespace TNY.BUS.Utils
{
    public class DBConnectionHelper
    {
        public MongoDatabase database;

        public DBConnectionHelper()
        {
            ConfigHelper config = new ConfigHelper();
            MongoClient client = new MongoClient(config.DBConnectionString);
            MongoServer server = client.GetServer();
            database = server.GetDatabase(config.DBName);
        }
    }
}
