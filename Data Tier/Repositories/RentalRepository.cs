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

    public async Task UpdateAsync(Rental rental)
    {
        var doc = _mapper.Map<RentalDocument>(rental);
        await _collection.ReplaceOneAsync(r => r.Id == doc.Id, doc);
    }

    public async Task UpdatePartialAsync(string id, RentalPatchDTO patchDto)
    {
        var updateBuilder = Builders<RentalDocument>.Update;
        var updateList = new List<UpdateDefinition<RentalDocument>>();

        if (patchDto.UserId is not null)
        {
            updateList.Add(updateBuilder.Set(r => r.UserId, patchDto.UserId));
        }

        if (patchDto.ScooterId is not null)
        {
            updateList.Add(updateBuilder.Set(r => r.ScooterId, patchDto.ScooterId));
        }

        if (patchDto.StartTime.HasValue)
        {
            updateList.Add(updateBuilder.Set(r => r.StartTime, patchDto.StartTime.Value));
        }

        if (patchDto.EndTime.HasValue)
        {
            updateList.Add(updateBuilder.Set(r => r.EndTime, patchDto.EndTime.Value));
        }

        if (patchDto.Status.HasValue)
        {
            updateList.Add(updateBuilder.Set(r => r.Status, patchDto.Status.Value));
        }

        if (patchDto.TotalCost.HasValue)
        {
            updateList.Add(updateBuilder.Set(r => r.TotalCost, patchDto.TotalCost.Value));
        }

        if (patchDto.CancellationReason is not null)
        {
            updateList.Add(updateBuilder.Set(r => r.CancellationReason, patchDto.CancellationReason));
        }

        if (updateList.Count == 0) return;

        var combinedUpdate = updateBuilder.Combine(updateList);
        await _collection.UpdateOneAsync(r => r.Id == id, combinedUpdate);
    }

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(r => r.Id == id);
}