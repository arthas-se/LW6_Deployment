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

    private MongoDBClient()
    {
        var connectionString = "mongodb+srv://mkoval706_db_user:j66sxVw4A6DuZupJ@lw4.bcdvpxx.mongodb.net/";
        var client = new MongoClient(connectionString);
        _db = client.GetDatabase("RESTfulAPI");
    }

}
