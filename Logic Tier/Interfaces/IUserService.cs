using Task2.Models.Domain;
using Task2.Models.DTO.Patch;
using Task2.Models.DTO.Post;

namespace Task2.Logic_Tier.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByLoginAsync(string login);
    Task<User> CreateAsync(UserPostDto dto);
    Task UpdateAsync(string id, User user);
    Task UpdatePartialAsync(string id, UserPatchDto patchDto);
    Task DeleteAsync(string id);
}
