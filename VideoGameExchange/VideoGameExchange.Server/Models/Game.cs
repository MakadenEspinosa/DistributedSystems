using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VideoGameExchange.Server.Models;

public class Game
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("platform")]
    public string Platform { get; set; } = string.Empty;
    
    [BsonElement("condition")]
    public string? Condition { get; set; }
    
    [BsonElement("year")]
    public int? Year { get; set; }
    
    [BsonElement("publisher")]
    public string? Publisher { get; set; }
    
    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;
}
