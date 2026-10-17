using Task2.Models.Domain;

namespace Task2.Models.DTO;

public class ScooterPatchDTO
{
    public int? Status { get; set; }
    public int? BatteryCharge { get; set; }
    public LocationCoordinate? Coordinate { get; set; }
    public decimal? Tariff { get; set; }
    public List<Rental>? RentalHistory { get; set; }
}
