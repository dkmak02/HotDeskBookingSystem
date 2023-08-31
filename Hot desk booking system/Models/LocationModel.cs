using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hot_desk_booking_system.Models;

public class LocationModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string LocationName { get; set; }
    public List<string> Desks { get; set; } = new List<string>();
}