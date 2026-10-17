using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Data_Tier.Interfaces;

public interface IScooterRepository
{
    Task<List<Scooter>> GetAllAsync();
    Task<Scooter?> GetByIdAsync(string id);
    Task CreateAsync(Scooter scooter);
    Task UpdateAsync(string id, Scooter scooter);
    Task DeleteAsync(string id);
}
