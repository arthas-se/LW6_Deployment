using Task2.Models.Domain;

namespace Task2.Logic_Tier.Interfaces;

public interface IJwtTokenGeneratorService
{
    string Generate(User user);

}
