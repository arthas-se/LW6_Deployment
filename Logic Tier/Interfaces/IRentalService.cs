using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Logic_Tier.Interfaces;

public interface IRentalService
{
    Task<List<Rental>> GetAllAsync();
    Task<Rental?> GetByIdAsync(string id);
    Task CreateAsync(Rental rental);
    Task UpdateAsync(string id, Rental rental);
    Task UpdatePartialAsync(string id, RentalPatchDTO patchDto);
    Task DeleteAsync(string id);
}
