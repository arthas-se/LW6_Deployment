using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Task2.Models.Documents;

public class RentalDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("scooterId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ScooterId { get; set; } = string.Empty;

    [BsonElement("startTime")]
    public DateTime StartTime { get; set; }

    [BsonElement("endTime")]
    public DateTime? EndTime { get; set; }

    [BsonElement("status")]
    public int Status { get; set; }

    [BsonElement("totalCost")]
    public decimal TotalCost { get; set; }

    [BsonElement("cancellationReason")]
    public string? CancellationReason { get; set; }
}
