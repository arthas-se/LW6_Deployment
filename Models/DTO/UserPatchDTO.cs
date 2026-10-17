using Task2.Models.Domain;

namespace Task2.Models.DTO;

public class UserPatchDTO
{
    public string? Login { get; set; }
    public decimal? Balance { get; set; }
    public decimal? BlockedDeposit { get; set; }
    public List<Rental>? RentalHistory { get; set; }
}
