using Task2.Models.Domain;

namespace Task2.Data_Tier.Interfaces;

public interface IScooterRepository
{
    Task<List<Scooter>> GetAllAsync();
    Task<Scooter?> GetByIdAsync(string id);
    Task CreateAsync(Scooter scooter);
    Task UpdateAsync(Scooter scooter);
    Task UpdatePartialAsync(string id, Dictionary<string, object> fieldsToUpdate);
    Task DeleteAsync(string id);
}
