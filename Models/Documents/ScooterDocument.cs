using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Task2.Models.Domain;

namespace Task2.Models.Documents;

public class LocationCoordinateDocument
{
    [BsonElement("latitude")]
    public double Latitude { get; set; }

    [BsonElement("longitude")]
    public double Longitude { get; set; }
}
public class ScooterDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("status")]
    public int Status { get; set; }

    [BsonElement("batteryCharge")]
    public int BatteryCharge { get; set; }

    [BsonElement("coordinate")]
    public LocationCoordinateDocument Coordinate { get; set; } = new();

    [BsonElement("tariff")]
    public decimal Tariff { get; set; }

    [BsonElement("rentalHistory")]
    public List<RentalDocument> RentalHistory { get; set; } = new();
}
