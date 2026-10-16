using AutoMapper;
using MongoDB.Driver;
using Task2.Data_Tier.DataBase;
using Task2.Data_Tier.Interfaces;
using Task2.Models.Documents;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Data_Tier.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _collection;
    private readonly IMapper _mapper;

    public UserRepository(IMapper mapper)
    {
        _collection = MongoDBClient.Instance.GetCollection<UserDocument>("Users");
        _mapper = mapper;
    }
    public async Task<List<User>> GetAllAsync()
    {
        var docs = await _collection.Find(_ => true).ToListAsync();
        return _mapper.Map<List<User>>(docs);
    }
    public async Task<User?> GetByIdAsync(string id)
    {
        var doc = await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
        return doc == null ? null : _mapper.Map<User>(doc);
    }
    public async Task CreateAsync(User user) =>
        await _collection.InsertOneAsync(_mapper.Map<UserDocument>(user));
    public async Task UpdateAsync(User user)
    {
        var doc = _mapper.Map<UserDocument>(user);
        await _collection.ReplaceOneAsync(u => u.Id == doc.Id, doc);
    }
    public async Task UpdatePartialAsync(string id, UserPatchDTO patchDto)
    {
        var updateBuilder = Builders<UserDocument>.Update;
        var updateList = new List<UpdateDefinition<UserDocument>>();

        if (patchDto.Login is not null)
        {
            updateList.Add(updateBuilder.Set(u => u.Login, patchDto.Login));
        }

        if (patchDto.Balance.HasValue)
        {
            updateList.Add(updateBuilder.Set(u => u.Balance, patchDto.Balance.Value));
        }

        if (patchDto.BlockedDeposit.HasValue)
        {
            updateList.Add(updateBuilder.Set(u => u.BlockedDeposit, patchDto.BlockedDeposit.Value));
        }

        if (patchDto.RentalHistory is not null)
        { 
            var mappedRentals = _mapper.Map<List<RentalDocument>>(patchDto.RentalHistory);
            updateList.Add(updateBuilder.Set(u => u.RentalHistory, mappedRentals));
        }
        
        if (updateList.Count == 0) return;
        var combinedUpdate = updateBuilder.Combine(updateList);

        await _collection.UpdateOneAsync(u => u.Id == id, combinedUpdate);
    }
    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(u => u.Id == id);

}
