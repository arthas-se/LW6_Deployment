using Task2.Data_Tier.Interfaces;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _repository;

    public RentalService(IRentalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Rental>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<Rental?> GetByIdAsync(string id) => await _repository.GetByIdAsync(id);
    public async Task CreateAsync(Rental rental) => await _repository.CreateAsync(rental);
    public async Task UpdateAsync(string id, Rental rental) => await _repository.UpdateAsync(id, rental);
    public async Task UpdatePartialAsync(string id, RentalPatchDTO patchDto)
    {
        var rental = await _repository.GetByIdAsync(id);
        if (rental == null)
        {
            throw new KeyNotFoundException($"Rental with ID {id} not found.");
        }

        if (patchDto.UserId is not null)
        {
            rental.UserId = patchDto.UserId;
        }

        if (patchDto.ScooterId is not null)
        {
            rental.ScooterId = patchDto.ScooterId;
        }

        if (patchDto.StartTime.HasValue)
        {
            rental.StartTime = patchDto.StartTime.Value;
        }

        if (patchDto.EndTime.HasValue)
        {
            rental.EndTime = patchDto.EndTime.Value;
        }

        if (patchDto.Status.HasValue)
        {
            rental.Status = (RentalStatus)patchDto.Status.Value; 
        }

        if (patchDto.TotalCost.HasValue)
        {
            rental.TotalCost = patchDto.TotalCost.Value;
        }

        if (patchDto.CancellationReason is not null)
        {
            rental.CancellationReason = patchDto.CancellationReason;
        }

        await _repository.UpdateAsync(id, rental);
    }
    public async Task DeleteAsync(string id) => await _repository.DeleteAsync(id);
}
