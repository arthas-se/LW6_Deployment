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
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; }
    public string ScooterId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Active;
    public decimal TotalCost { get; set; }
    public string? CancellationReason { get; set; }
}
