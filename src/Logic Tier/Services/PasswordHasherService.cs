using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using Task2.Logic_Tier.Interfaces;

namespace Task2.Logic_Tier.Services;

public class PasswordHasherService : IPasswordHasherService
{
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool Verify(string password, string hash)
    {
        var computed = HashPassword(password);
        return computed == hash;
    }
}
