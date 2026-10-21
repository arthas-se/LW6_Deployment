using System.ComponentModel.DataAnnotations;

namespace Task2.Models.Domain;

public enum UserRole
{
    Client,
    Admin
}
public class User
{
    public string Id { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Client;
    public decimal Balance { get; set; }
    public decimal BlockedDeposit { get; set; }
    public List<Rental> RentalHistory { get; set; } = new();
}
