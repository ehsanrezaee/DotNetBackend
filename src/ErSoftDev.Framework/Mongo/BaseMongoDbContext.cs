using ErSoftDev.Framework.BaseApp;
using MongoDB.Driver;

namespace ErSoftDev.Framework.Mongo
{
    public class BaseMongoDbContext
    {
        private readonly IMongoDatabase _database;
        public BaseMongoDbContext(AppSetting appSetting)
        {
            var client = new MongoClient(appSetting.ConnectionString.MongoConnectionString);
            var databaseName = MongoUrl.Create(appSetting.ConnectionString.MongoConnectionString).DatabaseName;
            _database = client.GetDatabase(databaseName);
        }
        public IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(typeof(T).Name);
        }
    }
}
