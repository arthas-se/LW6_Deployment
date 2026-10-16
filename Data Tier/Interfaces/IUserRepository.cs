using Task2.Models.Domain;

namespace Task2.Data_Tier.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task UpdatePartialAsync(string id, Dictionary<string, object> fieldsToUpdate);
    Task DeleteAsync(string id);
}
