using MongoDB.Driver;
namespace Hot_desk_booking_system.Services.MongoDB;

public interface IMongo
{
    IMongoCollection<T> Conn<T>(string collection);
}