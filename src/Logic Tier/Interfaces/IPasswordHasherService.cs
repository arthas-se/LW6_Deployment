namespace Task2.Logic_Tier.Interfaces;

public interface IPasswordHasherService
{
    string HashPassword(string password);
    bool Verify(string password, string hash);
}
