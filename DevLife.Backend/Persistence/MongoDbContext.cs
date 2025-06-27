using DevLife.Backend.Domain;
using MongoDB.Driver;

namespace DevLife.Backend.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration config)
    {
        var connectionString = config["Mongo:ConnectionString"];
        var dbName = config["Mongo:Database"];
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(dbName);
    }

    public IMongoCollection<CodeSnippet> Snippets =>
        _database.GetCollection<CodeSnippet>("CodeSnippets");

    public IMongoCollection<DatingProfile> DatingProfiles =>
        _database.GetCollection<DatingProfile>("DatingProfiles");


}
