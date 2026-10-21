namespace Task2.Models.DTO;

public class CancelRentalDto
{
    public string RentalId { get; set; } = string.Empty;
    public string CancellationReason { get; set; } = string.Empty;
}
