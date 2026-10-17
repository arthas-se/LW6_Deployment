using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Data_Tier.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task CreateAsync(User user);
    Task UpdateAsync(string id, User user);
    Task DeleteAsync(string id);
}
