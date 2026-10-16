using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Task2.Models.Domain;

namespace Task2.Models.Documents;

public class UserDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("login")]
    public string Login { get; set; } = string.Empty;

    [BsonElement("balance")]
    public decimal Balance { get; set; }

    [BsonElement("blockedDeposit")]
    public decimal BlockedDeposit { get; set; }

    [BsonElement("rentalHistory")]
    public List<RentalDocument> RentalHistory { get; set; } = new();
}
