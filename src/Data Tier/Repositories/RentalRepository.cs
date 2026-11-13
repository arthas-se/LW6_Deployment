using AutoMapper;
using MongoDB.Driver;
using Task2.Data_Tier.DataBase;
using Task2.Data_Tier.Interfaces;
using Task2.Models.Documents;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Data_Tier.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly IMongoCollection<RentalDocument> _collection;
    private readonly IMapper _mapper;

    public RentalRepository(IMapper mapper)
    {
        _collection = MongoDBClient.Instance.GetCollection<RentalDocument>("Rentals");
        _mapper = mapper;
    }

    public async Task<List<Rental>> GetAllAsync()
    {
        var docs = await _collection.Find(_ => true).ToListAsync();
        return _mapper.Map<List<Rental>>(docs);
    }

    public async Task<Rental?> GetByIdAsync(string id)
    {
        var doc = await _collection.Find(r => r.Id == id).FirstOrDefaultAsync();
        return doc == null ? null : _mapper.Map<Rental>(doc);
    }

    public async Task CreateAsync(Rental rental) =>
        await _collection.InsertOneAsync(_mapper.Map<RentalDocument>(rental));

    public async Task UpdateAsync(string id, Rental rental)
    {
        var doc = _mapper.Map<RentalDocument>(rental);
        doc.Id = id;
        await _collection.ReplaceOneAsync(r => r.Id == doc.Id, doc);
    }
    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(r => r.Id == id);
}