using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hot_desk_booking_system.Models;

public class DeskModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string DeskName { get; set; }
    public string LocationName { get; set; }
}