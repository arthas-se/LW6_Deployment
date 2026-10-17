using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Logic_Tier.Interfaces;

public interface IScooterService
{
    Task<List<Scooter>> GetAllAsync();
    Task<Scooter?> GetByIdAsync(string id);
    Task CreateAsync(Scooter scooter);
    Task UpdateAsync(string id, Scooter scooter);
    Task UpdatePartialAsync(string id, ScooterPatchDTO patchDto);
    Task DeleteAsync(string id);
}
