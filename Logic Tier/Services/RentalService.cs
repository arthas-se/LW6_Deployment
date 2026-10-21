using Task2.Data_Tier.Interfaces;
using Task2.Data_Tier.Repositories;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IScooterRepository _scooterRepository;
    private readonly IUserRepository _userRepository;

    public RentalService(
        IRentalRepository rentalRepository,
        IScooterRepository scooterRepository,
        IUserRepository userRepository)
    {
        _rentalRepository = rentalRepository;
        _scooterRepository = scooterRepository;
        _userRepository = userRepository;
    }

    public async Task<List<Rental>> GetAllAsync() => await _rentalRepository.GetAllAsync();
    public async Task<Rental?> GetByIdAsync(string id) => await _rentalRepository.GetByIdAsync(id);
    public async Task CreateAsync(Rental rental)
    {
        rental.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        await _rentalRepository.CreateAsync(rental);
    }
    public async Task UpdateAsync(string id, Rental rental) => await _rentalRepository.UpdateAsync(id, rental);
    public async Task UpdatePartialAsync(string id, RentalPatchDto patchDto)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
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

        await _rentalRepository.UpdateAsync(id, rental);
    }
    public async Task DeleteAsync(string id) => await _rentalRepository.DeleteAsync(id);

    public async Task<Rental> StartRentalAsync(StartRentalDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {dto.UserId} not found.");

        var scooter = await _scooterRepository.GetByIdAsync(dto.ScooterId);
        if (scooter == null)
            throw new KeyNotFoundException($"Scooter with ID {dto.ScooterId} not found.");

        if (scooter.Status != ScooterStatus.Available || scooter.BatteryCharge < 20)
        {
            throw new InvalidOperationException("Scooter is not available for rental or battery level is below 20%.");
        }

        var rentals = await _rentalRepository.GetAllAsync();
        var activeRentals = rentals.Where(r => r.UserId == dto.UserId && r.Status == RentalStatus.Active).ToList();

        if (activeRentals != null && activeRentals.Count > 0)
        {
            throw new InvalidOperationException("User already has an active rental.");
        }

        const decimal depositAmount = 50.0m;
        if (user.Balance < depositAmount)
        {
            throw new InvalidOperationException("Insufficient user balance. Minimum 50 UAH required for deposit.");
        }

        user.Balance -= depositAmount;

        var newRental = new Rental
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            UserId = user.Id,
            ScooterId = scooter.Id,
            StartTime = DateTime.UtcNow,
            Status = RentalStatus.Active,
            TotalCost = depositAmount
        };

        user.RentalHistory ??= new List<Rental>();
        user.RentalHistory.Add(newRental); 

        scooter.Status = ScooterStatus.InUse;
        scooter.RentalHistory ??= new List<Rental>();
        scooter.RentalHistory.Add(newRental);

        await _userRepository.UpdateAsync(user.Id, user);
        await _scooterRepository.UpdateAsync(scooter.Id, scooter);
        await _rentalRepository.CreateAsync(newRental);

        return newRental;
    }
    public async Task<Rental> FinishRentalAsync(FinishRentalDto dto)
    {
        var id = dto.RentalId;
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
            throw new KeyNotFoundException($"Rental with ID {id} not found.");

        if (rental.Status != RentalStatus.Active)
        {
            throw new InvalidOperationException("Only active rentals can be finished.");
        }

        var scooter = await _scooterRepository.GetByIdAsync(rental.ScooterId);
        if (scooter == null)
            throw new KeyNotFoundException($"Scooter associated with rental not found.");

        var user = await _userRepository.GetByIdAsync(rental.UserId);
        if (user == null)
            throw new KeyNotFoundException($"User associated with rental not found.");

        rental.EndTime = DateTime.UtcNow;

        var durationMinutes = (int)Math.Ceiling((rental.EndTime.Value - rental.StartTime).TotalMinutes);
        if (durationMinutes < 1) durationMinutes = 1;

        decimal baseCost = 10.0m + (durationMinutes * scooter.Tariff);
        if (durationMinutes > 60)
        {
            baseCost *= 0.85m;
        }

        rental.TotalCost = Math.Round(baseCost, 2);

        const decimal depositAmount = 50.0m;
        user.Balance += depositAmount;

        if (user.Balance < rental.TotalCost)
        {
            throw new InvalidOperationException("User balance is insufficient to cover the final rental cost.");
        }

        user.Balance -= rental.TotalCost;

        if (scooter.BatteryCharge < 15)
        {
            scooter.Status = ScooterStatus.LowBattery;
        }
        else
        {
            scooter.Status = ScooterStatus.Available;
        }

        rental.Status = RentalStatus.Completed;

        if (user.RentalHistory != null)
        {
            var userRental = user.RentalHistory.FirstOrDefault(r => r.Id == rental.Id);
            if (userRental != null)
            {
                userRental.EndTime = rental.EndTime;
                userRental.TotalCost = rental.TotalCost;
                userRental.Status = rental.Status;
            }
        }

        if (scooter.RentalHistory != null)
        {
            var scooterRental = scooter.RentalHistory.FirstOrDefault(r => r.Id == rental.Id);
            if (scooterRental != null)
            {
                scooterRental.EndTime = rental.EndTime;
                scooterRental.TotalCost = rental.TotalCost;
                scooterRental.Status = rental.Status;
            }
        }

        await _userRepository.UpdateAsync(user.Id, user);
        await _scooterRepository.UpdateAsync(scooter.Id, scooter);
        await _rentalRepository.UpdateAsync(rental.Id, rental);

        return rental;
    }
    public async Task CancelRentalAsync(CancelRentalDto dto)
    {
        var rental = await _rentalRepository.GetByIdAsync(dto.RentalId);
        if (rental == null)
            throw new KeyNotFoundException($"Rental with ID {dto.RentalId} not found.");

        if (rental.Status != RentalStatus.Active)
        {
            throw new InvalidOperationException("Only active rentals can be cancelled.");
        }

        var elapsedMinutes = (DateTime.UtcNow - rental.StartTime).TotalMinutes;
        if (elapsedMinutes > 2.0)
        {
            throw new InvalidOperationException("Rental can only be cancelled within the first 2 minutes of the start.");
        }

        rental.Status = RentalStatus.Canceled;
        rental.CancellationReason = string.IsNullOrEmpty(dto.CancellationReason) ? "Not specified" : dto.CancellationReason;
        rental.EndTime = DateTime.UtcNow;
        rental.TotalCost = 0;

        var user = await _userRepository.GetByIdAsync(rental.UserId);
        if (user != null)
        {
            user.Balance += 50.0m;

            if (user.RentalHistory != null)
            {
                var userRental = user.RentalHistory.FirstOrDefault(r => r.Id == rental.Id);
                if (userRental != null)
                {
                    userRental.Status = rental.Status;
                    userRental.CancellationReason = rental.CancellationReason;
                    userRental.EndTime = rental.EndTime;
                    userRental.TotalCost = 0;
                }
            }

            await _userRepository.UpdateAsync(user.Id, user);
        }

        var scooter = await _scooterRepository.GetByIdAsync(rental.ScooterId);
        if (scooter != null)
        {
            scooter.Status = ScooterStatus.Maintenance;

            if (scooter.RentalHistory != null)
            {
                var scooterRental = scooter.RentalHistory.FirstOrDefault(r => r.Id == rental.Id);
                if (scooterRental != null)
                {
                    scooterRental.Status = rental.Status;
                    scooterRental.CancellationReason = rental.CancellationReason;
                    scooterRental.EndTime = rental.EndTime;
                    scooterRental.TotalCost = 0;
                }
            }

            await _scooterRepository.UpdateAsync(scooter.Id, scooter);
        }

        await _rentalRepository.UpdateAsync(rental.Id, rental);
    }
}
