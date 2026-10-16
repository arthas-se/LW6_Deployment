using Task2.Models.Domain;

namespace Task2.Data_Tier.Interfaces;

public interface IRentalRepository
{
    Task<List<Rental>> GetAllAsync();
    Task<Rental?> GetByIdAsync(string id);
    Task CreateAsync(Rental scooter);
    Task UpdateAsync(Rental scooter);
    Task UpdatePartialAsync(string id, Dictionary<string, object> fieldsToUpdate);
    Task DeleteAsync(string id);
}
