using AutoMapper;
using MongoDB.Driver;
using Task2.Data_Tier.DataBase;
using Task2.Models.Documents;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Data_Tier.Repositories;

public class ScooterRepository
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
    public async Task UpdateAsync(Scooter scooter)
    {
        var doc = _mapper.Map<ScooterDocument>(scooter);
        await _collection.ReplaceOneAsync(s => s.Id == doc.Id, doc);
    }
    public async Task UpdatePartialAsync(string id, ScooterPatchDTO patchDto)
    {
        var updateBuilder = Builders<ScooterDocument>.Update;
        var updateList = new List<UpdateDefinition<ScooterDocument>>();

        if (patchDto.Status.HasValue)
        {
            updateList.Add(updateBuilder.Set(s => s.Status, patchDto.Status));
        }

        if (patchDto.BatteryCharge.HasValue)
        {
            updateList.Add(updateBuilder.Set(s => s.BatteryCharge, patchDto.BatteryCharge));
        }

        if (patchDto.Coordinate is not null)
        {
            var mappedLocation = _mapper.Map<LocationCoordinateDocument>(patchDto.Coordinate);
            updateList.Add(updateBuilder.Set(s => s.Coordinate, mappedLocation));
        }

        if (patchDto.Tariff.HasValue)
        {
            updateList.Add(updateBuilder.Set(s => s.Tariff, patchDto.Tariff));
        }

        if (patchDto.RentalHistory is not null)
        {
            var mappedRentals = _mapper.Map<List<RentalDocument>>(patchDto.RentalHistory);
            updateList.Add(updateBuilder.Set(s => s.RentalHistory, mappedRentals));
        }

        if (updateList.Count == 0) return;
        var combinedUpdate = updateBuilder.Combine(updateList);

        await _collection.UpdateOneAsync(s => s.Id == id, combinedUpdate);
    }
    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(u => u.Id == id);
}
