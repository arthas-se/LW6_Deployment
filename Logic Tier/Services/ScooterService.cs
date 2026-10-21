using Task2.Data_Tier.Interfaces;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Services;

public class ScooterService : IScooterService
{
    private readonly IScooterRepository _repository;

    public ScooterService(IScooterRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Scooter>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<Scooter?> GetByIdAsync(string id) => await _repository.GetByIdAsync(id);
    public async Task CreateAsync(Scooter scooter)
    {
        scooter.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        await _repository.CreateAsync(scooter);
    } 
    public async Task UpdateAsync(string id, Scooter scooter) => await _repository.UpdateAsync(id, scooter);
    public async Task UpdatePartialAsync(string id, ScooterPatchDto patchDto)
    {
        var scooter = await _repository.GetByIdAsync(id);
        if (scooter == null)
        {
            throw new KeyNotFoundException($"Scooter with ID {id} not found.");
        }

        if (patchDto.Status.HasValue)
        {
            scooter.Status = (ScooterStatus)patchDto.Status.Value;
        }

        if (patchDto.BatteryCharge.HasValue)
        {
            scooter.BatteryCharge = patchDto.BatteryCharge.Value;
        }

        if (patchDto.Coordinate is not null)
        {
            scooter.Coordinate = patchDto.Coordinate;
        }

        if (patchDto.Tariff.HasValue)
        {
            scooter.Tariff = patchDto.Tariff.Value;
        }

        if (patchDto.RentalHistory is not null)
        {
            scooter.RentalHistory = patchDto.RentalHistory;
        }

        await _repository.UpdateAsync(id, scooter);
    }
    public async Task DeleteAsync(string id) => await _repository.DeleteAsync(id);
}
