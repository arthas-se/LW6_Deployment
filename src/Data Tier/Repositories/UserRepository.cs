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
    public async Task<User?> GetByLoginAsync(string login)
    {
        var doc = await _collection.Find(u => u.Login == login).FirstOrDefaultAsync();
        return doc == null ? null : _mapper.Map<User>(doc);
    }
    public async Task CreateAsync(User user) =>
        await _collection.InsertOneAsync(_mapper.Map<UserDocument>(user));
    public async Task UpdateAsync(string id, User user)
    {
        var doc = _mapper.Map<UserDocument>(user);
        doc.Id = id;
        await _collection.ReplaceOneAsync(u => u.Id == id, doc);
    }
    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(u => u.Id == id);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.RefreshToken, refreshToken);
        var userDoc = await _collection.Find(filter).FirstOrDefaultAsync();

        if (userDoc == null)
            return null;

        return _mapper.Map<User>(userDoc);
    }

}
