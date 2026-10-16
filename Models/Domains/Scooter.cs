using System.Text.Json.Serialization;

namespace Task2.Models.Domain;
public enum ScooterStatus
{
    Available,
    InUse,
    LowBattery,
    Maintenance
}

public class LocationCoordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class Scooter
{
    public int Id { get; set; }
    public ScooterStatus Status { get; set; } = ScooterStatus.Available;
    public int BatteryCharge { get; set; }
    public LocationCoordinate Coordinate { get; set; } = new();
    public decimal Tariff { get; set; } = 3m;
    public List<Rental> RentalHistory { get; set; } = new();
}
