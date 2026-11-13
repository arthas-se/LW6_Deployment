using MongoDB.Driver;

namespace Task2.Data_Tier.DataBase;

public class MongoDBClient
{
    private static MongoDBClient _instance;
    private readonly IMongoDatabase _db;

    public static MongoDBClient Instance
    {
        get => _instance ?? throw new InvalidOperationException("MongoDBClient is not initialized via DI yet.");
    }

    public MongoDBClient(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _db = client.GetDatabase(databaseName);

        _instance = this;
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _db.GetCollection<T>(name);
}