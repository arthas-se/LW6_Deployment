using Task2.Data_Tier.Interfaces;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<User>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<User?> GetByIdAsync(string id) => await _repository.GetByIdAsync(id);
    public async Task CreateAsync(User user) 
    {
        user.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        await _repository.CreateAsync(user);
    }
    public async Task UpdateAsync(string id, User user) => await _repository.UpdateAsync(id, user);
    public async Task UpdatePartialAsync(string id, UserPatchDto patchDto)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        if (patchDto.Login is not null)
        {
            user.Login = patchDto.Login;
        }

        if (patchDto.Balance.HasValue)
        {
            user.Balance = patchDto.Balance.Value;
        }

        if (patchDto.BlockedDeposit.HasValue)
        {
            user.BlockedDeposit = patchDto.BlockedDeposit.Value;
        }

        if (patchDto.RentalHistory is not null)
        {
            user.RentalHistory = patchDto.RentalHistory;
        }

        await _repository.UpdateAsync(id, user);
    }
    public async Task DeleteAsync(string id) => await _repository.DeleteAsync(id);
}
