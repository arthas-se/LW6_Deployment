using System.Text.Json.Serialization;

namespace Task2.Models.Domain;

public enum RentalStatus
{
    Active,
    Completed,
    Canceled
}

public class Rental
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ScooterId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Active;
    public decimal TotalCost { get; set; }
    public string? CancellationReason { get; set; }
}
