using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Logic_Tier.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task CreateAsync(User user);
    Task UpdateAsync(string id, User user);
    Task UpdatePartialAsync(string id, UserPatchDto patchDto);
    Task DeleteAsync(string id);
}
