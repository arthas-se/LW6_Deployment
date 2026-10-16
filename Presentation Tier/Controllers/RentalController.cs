//RentalController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task2.Models.Domain;
using Task2.Utils;

namespace Task2.Controllers;

public class StartRentalDto
{
    public int UserId { get; set; }
    public int ScooterId { get; set; }
}

[Route("api/rental")]
[ApiController]
public class RentalController : ControllerBase
{
    private readonly JsonFileReader _reader;
    private readonly JsonFileWriter _writer;
    private const string FileName = "rentals.json";

    public RentalController(JsonFileReader reader, JsonFileWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRentals()
    {
        var rentals = await _reader.ReadDataAsync<Rental>(FileName);
        return Ok(rentals);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var rentals = await _reader.ReadDataAsync<Rental>(FileName);
        var rental = rentals.FirstOrDefault(r => r.Id == id);

        if (rental == null)
            return NotFound("Rental not found!");

        return Ok(rental);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Rental newRental)
    {
        var rentals = await _reader.ReadDataAsync<Rental>(FileName);

        newRental.Id = rentals.Any() ? rentals.Max(r => r.Id) + 1 : 1;
        rentals.Add(newRental);

        await _writer.WriteDataAsync(FileName, rentals);

        return CreatedAtAction(nameof(GetById), new { id = newRental.Id }, newRental);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Rental updatedRental)
    {
        var rentals = await _reader.ReadDataAsync<Rental>(FileName);
        var index = rentals.FindIndex(r => r.Id == id);

        if (index == -1)
            return NotFound($"Rental with ID={id} not found!");

        updatedRental.Id = id;
        rentals[index] = updatedRental;

        await _writer.WriteDataAsync(FileName, rentals);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] Dictionary<string, object> updatedRental)
    {
        var rentals = await _reader.ReadDataAsync<Rental>(FileName);
        var rental = rentals.FirstOrDefault(r => r.Id == id);

        if (rental == null)
            return NotFound($"Rental with ID={id} not found!");

        var changes = new Dictionary<string, object>(updatedRental, StringComparer.OrdinalIgnoreCase);

        if (changes.TryGetValue("status", out var statusVal) && statusVal != null && Enum.TryParse<RentalStatus>(statusVal.ToString(), true, out var status))
        {
            rental.Status = status;
        }

        if (changes.TryGetValue("endTime", out var endTimeVal))
        {
            if (endTimeVal == null)
            {
                rental.EndTime = null;
            }
            else if (DateTime.TryParse(endTimeVal.ToString(), out var endTime))
            {
                rental.EndTime = endTime;
            }
        }

        if (changes.TryGetValue("totalCost", out var costVal) && decimal.TryParse(costVal?.ToString(), out var cost))
        {
            rental.TotalCost = cost;
        }

        await _writer.WriteDataAsync(FileName, rentals);

        return Ok(rental);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(int id)
    {
        var rentals = await _reader.ReadDataAsync<Rental>(FileName);
        var index = rentals.FindIndex(r => r.Id == id);

        if (index == -1)
            return NotFound($"Rental with ID={id} not found!");

        rentals.RemoveAt(index);
        await _writer.WriteDataAsync(FileName, rentals);
        return NoContent();
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartRental([FromBody] StartRentalDto dto)
    {
        var users = await _reader.ReadDataAsync<User>("users.json");
        var scooters = await _reader.ReadDataAsync<Scooter>("scooters.json");
        var rentals = await _reader.ReadDataAsync<Rental>("rentals.json");

        var user = users.FirstOrDefault(u => u.Id == dto.UserId);
        if (user == null)
            return NotFound("User not found.");

        var scooter = scooters.FirstOrDefault(s => s.Id == dto.ScooterId);
        if (scooter == null)
            return NotFound("Scooter not found.");

        if (scooter.Status != ScooterStatus.Available)
            return BadRequest("Scooter can't be rented.");

        if (scooter.BatteryCharge < 20)
            return BadRequest("Scooter charge is to low");

        var hasActiveRental = rentals.Any(r => r.UserId == dto.UserId && r.Status == RentalStatus.Active);
        if (hasActiveRental)
            return BadRequest("User already have rental!");

        if (user.Balance < 50m)
            return BadRequest("Balance is yo low! Minimal deposit - 50 UAH");

        user.Balance -= 50m;
        user.BlockedDeposit += 50m;

        scooter.Status = ScooterStatus.InUse;

        var newRental = new Rental
        {
            Id = rentals.Any() ? rentals.Max(r => r.Id) + 1 : 1,
            UserId = user.Id,
            ScooterId = scooter.Id,
            StartTime = DateTime.UtcNow,
            Status = RentalStatus.Active,
            TotalCost = 0m
        };

        rentals.Add(newRental);
        user.RentalHistory.Add(newRental);
        scooter.RentalHistory.Add(newRental);

        await _writer.WriteDataAsync("users.json", users);
        await _writer.WriteDataAsync("scooters.json", scooters);
        await _writer.WriteDataAsync("rentals.json", rentals);

        return CreatedAtAction(nameof(GetById), new { id = newRental.Id }, newRental);
    }
    [HttpPost("finish/{id}")]
    public async Task<IActionResult> FinishRental(int id)
    {
        var rentals = await _reader.ReadDataAsync<Rental>("rentals.json");
        var rental = rentals.FirstOrDefault(r => r.Id == id);

        if (rental == null)
            return NotFound("Rental not found.");

        if (rental.Status != RentalStatus.Active)
            return BadRequest("Cac only finish active rental.");

        var users = await _reader.ReadDataAsync<User>("users.json");
        var scooters = await _reader.ReadDataAsync<Scooter>("scooters.json");

        var user = users.FirstOrDefault(u => u.Id == rental.UserId);
        var scooter = scooters.FirstOrDefault(s => s.Id == rental.ScooterId);

        if (user == null || scooter == null)
            return NotFound("User or scooter don't found.");

        var endTime = DateTime.UtcNow;
        rental.EndTime = endTime;

        var durationMinutes = (int)Math.Ceiling((endTime - rental.StartTime).TotalMinutes);
        if (durationMinutes < 1) durationMinutes = 1;

        decimal cost = 10m + (durationMinutes * scooter.Tariff);

        if (durationMinutes > 60)
        {
            cost *= 0.85m;
        }

        rental.TotalCost = Math.Round(cost, 2);
        rental.Status = RentalStatus.Completed;

        user.BlockedDeposit = Math.Max(0m, user.BlockedDeposit - 50m);
        user.Balance += 50m;
        user.Balance -= rental.TotalCost;

        scooter.Status = scooter.BatteryCharge < 15 ? ScooterStatus.LowBattery : ScooterStatus.Available;

        UpdateHistoryItem(user.RentalHistory, rental);
        UpdateHistoryItem(scooter.RentalHistory, rental);

        await _writer.WriteDataAsync("users.json", users);
        await _writer.WriteDataAsync("scooters.json", scooters);
        await _writer.WriteDataAsync("rentals.json", rentals);

        return Ok(rental.TotalCost);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelRental(int id, [FromBody] Dictionary<string, string> body)
    {
        if (!body.TryGetValue("reason", out var reason) || string.IsNullOrWhiteSpace(reason))
            return BadRequest("Reason of cancelling is required.");

        var rentals = await _reader.ReadDataAsync<Rental>("rentals.json");
        var rental = rentals.FirstOrDefault(r => r.Id == id);

        if (rental == null)
            return NotFound("Rental not found.");

        if (rental.Status != RentalStatus.Active)
            return BadRequest("Cac only cancel active rental.");

        var elapsedMinutes = (DateTime.UtcNow - rental.StartTime).TotalMinutes;
        if (elapsedMinutes > 2)
            return BadRequest("The fare is refunded only within the first 2 minutes of the trip.");

        var users = await _reader.ReadDataAsync<User>("users.json");
        var scooters = await _reader.ReadDataAsync<Scooter>("scooters.json");

        var user = users.FirstOrDefault(u => u.Id == rental.UserId);
        var scooter = scooters.FirstOrDefault(s => s.Id == rental.ScooterId);

        if (user == null || scooter == null)
            return NotFound("User or scooter don't found.");

        user.BlockedDeposit = Math.Max(0m, user.BlockedDeposit - 50m);
        user.Balance += 50m;

        scooter.Status = ScooterStatus.Maintenance;

        rental.EndTime = DateTime.UtcNow;
        rental.Status = RentalStatus.Canceled;
        rental.TotalCost = 0m;
        rental.CancellationReason = reason;

        UpdateHistoryItem(user.RentalHistory, rental);
        UpdateHistoryItem(scooter.RentalHistory, rental);

        await _writer.WriteDataAsync("users.json", users);
        await _writer.WriteDataAsync("scooters.json", scooters);
        await _writer.WriteDataAsync("rentals.json", rentals);

        return Ok(rental);
    }

    private static void UpdateHistoryItem(List<Rental> history, Rental updatedRental)
    {
        var index = history.FindIndex(r => r.Id == updatedRental.Id);
        if (index != -1)
        {
            history[index] = updatedRental;
        }
        else
        {
            history.Add(updatedRental);
        }
    }
}
