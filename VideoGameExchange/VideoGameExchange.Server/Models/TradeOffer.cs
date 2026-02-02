using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VideoGameExchange.Server.Models;

public class TradeOffer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("offerorUserId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string OfferorUserId { get; set; } = string.Empty;
    
    [BsonElement("offereeUserId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string OffereeUserId { get; set; } = string.Empty;
    
    [BsonElement("offeredGameIds")]
    public List<string> OfferedGameIds { get; set; } = new();
    
    [BsonElement("requestedGameIds")]
    public List<string> RequestedGameIds { get; set; } = new();
    
    [BsonElement("status")]
    public string Status { get; set; } = "pending"; // pending, accepted, rejected, completed
    
    [BsonElement("message")]
    public string? Message { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
