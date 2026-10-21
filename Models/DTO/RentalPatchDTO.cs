using Task2.Models.Domain;

namespace Task2.Models.DTO;

public class RentalPatchDto
{
    public string? UserId { get; set; }
    public string? ScooterId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? Status { get; set; }
    public string? CancellationReason { get; set; }
}
