using AutoMapper;
using Task2.Data_Tier.Interfaces;
using Task2.Data_Tier.Repositories;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO.Patch;
using Task2.Models.DTO.Post;

namespace Task2.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<User>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<User?> GetByIdAsync(string id) => await _repository.GetByIdAsync(id);
    public async Task<User?> GetByLoginAsync(string login) => await _repository.GetByLoginAsync(login);
    public async Task<User> CreateAsync(UserPostDto dto)
    {
        var user = _mapper.Map<User>(dto);
        await _repository.CreateAsync(user);
        return user;
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

        if (patchDto.Password is not null)
        {
            user.Password = patchDto.Password;
        }

        if (patchDto.Balance.HasValue)
        {
            user.Balance = patchDto.Balance.Value;
        }

        if (patchDto.BlockedDeposit.HasValue)
        {
            user.BlockedDeposit = patchDto.BlockedDeposit.Value;
        }

        await _repository.UpdateAsync(id, user);
    }
    public async Task DeleteAsync(string id) => await _repository.DeleteAsync(id);
}
