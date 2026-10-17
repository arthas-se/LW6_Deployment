using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Data_Tier.Interfaces;

public interface IRentalRepository
{
    Task<List<Rental>> GetAllAsync();
    Task<Rental?> GetByIdAsync(string id);
    Task CreateAsync(Rental rental);
    Task UpdateAsync(string id, Rental rental);
    Task DeleteAsync(string id);
}
