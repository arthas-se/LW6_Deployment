using Task2.Models.Domain;

namespace Task2.Models.DTO;

public class UserPatchDto
{
    public string? Login { get; set; }
    public decimal? Balance { get; set; }
    public decimal? BlockedDeposit { get; set; }
}
