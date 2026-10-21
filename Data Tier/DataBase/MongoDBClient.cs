using MongoDB.Driver;

namespace Task2.Data_Tier.DataBase;

public class MongoDBClient
{
    private static IMongoDatabase _db;
    private static MongoDBClient _instance;

    public static MongoDBClient Instance
    {
        get => _instance ?? new MongoDBClient();
    }

    public static void Initialize(string connectionString, string databaseName)
    {
        if (_instance == null)
        {
            var client = new MongoClient(connectionString);
            _db = client.GetDatabase(databaseName);
            _instance = new MongoDBClient();
        }
    }
    private MongoDBClient()
    {
        
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _db.GetCollection<T>(name);
}
