using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogServer.Models;

public class Blog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string PhotoUrl { get; set; }
    public string? UserId { get; set; }
    public DateTime CreatedDate { get; set; }
}

