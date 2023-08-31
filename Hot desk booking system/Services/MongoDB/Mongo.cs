using MongoDB.Driver;

namespace Hot_desk_booking_system.Services.MongoDB;

public class Mongo : IMongo
{
    private readonly IConfiguration _configuration;
    public Mongo(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public IMongoCollection<T> Conn<T>(string collection)
    {
        var client = new MongoClient(_configuration.GetConnectionString("DBConnectionString"))
            .GetDatabase(_configuration.GetConnectionString("DBName"));
        return client.GetCollection<T>(collection);
    }
}