using System.ComponentModel.DataAnnotations;

namespace Task2.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal BlockedDeposit { get; set; }
    public List<Rental> RentalHistory { get; set; } = new();
}
