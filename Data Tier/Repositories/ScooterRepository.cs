using AutoMapper;
using MongoDB.Driver;
using Task2.Data_Tier.DataBase;
using Task2.Data_Tier.Interfaces;
using Task2.Models.Documents;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Data_Tier.Repositories;

public class ScooterRepository : IScooterRepository
{
    private readonly IMongoCollection<ScooterDocument> _collection;
    private readonly IMapper _mapper;

    public ScooterRepository(IMapper mapper)
    {
        _collection = MongoDBClient.Instance.GetCollection<ScooterDocument>("Scooter");
        _mapper = mapper;
    }
    public async Task<List<Scooter>> GetAllAsync()
    {
        var docs = await _collection.Find(_ => true).ToListAsync();
        return _mapper.Map<List<Scooter>>(docs);
    }
    public async Task<Scooter?> GetByIdAsync(string id)
    {
        var doc = await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();
        return doc == null ? null : _mapper.Map<Scooter>(doc);
    }
    public async Task CreateAsync(Scooter scooter) =>
        await _collection.InsertOneAsync(_mapper.Map<ScooterDocument>(scooter));
    public async Task UpdateAsync(string id, Scooter scooter)
    {
        var doc = _mapper.Map<ScooterDocument>(scooter);
        doc.Id = id;
        await _collection.ReplaceOneAsync(s => s.Id == id, doc);
    }
    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(u => u.Id == id);
}
