using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hot_desk_booking_system.Models;

public class BookingModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string DeskId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    
    public string UserId { get; set; }
    
}