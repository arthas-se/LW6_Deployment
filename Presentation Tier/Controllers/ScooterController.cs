//ScooterController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task2.Models.Domain;
using Task2.Utils;

namespace Task2.Controllers;

[Route("api/scooter")]
[ApiController]
public class ScooterController : ControllerBase
{
    private readonly JsonFileReader _reader;
    private readonly JsonFileWriter _writer;
    private const string FileName = "scooters.json";

    public ScooterController(JsonFileReader reader, JsonFileWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllScooters()
    {
        var scooters = await _reader.ReadDataAsync<Scooter>(FileName);
        return Ok(scooters);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var scooters = await _reader.ReadDataAsync<Scooter>(FileName);
        var scooter = scooters.FirstOrDefault(s => s.Id == id);

        if (scooter == null)
            return NotFound("Scooter not found!");

        return Ok(scooter);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Scooter newScooter)
    {
        var scooters = await _reader.ReadDataAsync<Scooter>(FileName);

        newScooter.Id = scooters.Any() ? scooters.Max(s => s.Id) + 1 : 1;
        scooters.Add(newScooter);

        await _writer.WriteDataAsync(FileName, scooters);

        return CreatedAtAction(nameof(GetById), new { id = newScooter.Id }, newScooter);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Scooter updatedScooter)
    {
        var scooters = await _reader.ReadDataAsync<Scooter>(FileName);
        var index = scooters.FindIndex(s => s.Id == id);

        if (index == -1)
            return NotFound($"Scooter with ID={id} not found!");

        updatedScooter.Id = id;
        scooters[index] = updatedScooter;

        await _writer.WriteDataAsync(FileName, scooters);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] Dictionary<string, object> updatedScooter)
    {
        var scooters = await _reader.ReadDataAsync<Scooter>(FileName);
        var scooter = scooters.FirstOrDefault(s => s.Id == id);

        if (scooter == null)
            return NotFound($"Scooter with ID={id} not found!");

        var changes = new Dictionary<string, object>(updatedScooter, StringComparer.OrdinalIgnoreCase);

        if (changes.TryGetValue("status", out var statusVal) && statusVal != null && Enum.TryParse<ScooterStatus>(statusVal.ToString(), true, out var status))
        {
            scooter.Status = status;
        }

        if (changes.TryGetValue("batteryCharge", out var batteryVal) && int.TryParse(batteryVal?.ToString(), out var battery))
        {
            scooter.BatteryCharge = battery;
        }

        if (changes.TryGetValue("tariff", out var tariffVal) && decimal.TryParse(tariffVal?.ToString(), out var tariff))
        {
            scooter.Tariff = tariff;
        }

        await _writer.WriteDataAsync(FileName, scooters);

        return Ok(scooter);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(int id)
    {
        var scooters = await _reader.ReadDataAsync<Scooter>(FileName);
        var index = scooters.FindIndex(s => s.Id == id);

        if (index == -1)
            return NotFound($"Scooter with ID={id} not found!");

        scooters.RemoveAt(index);
        await _writer.WriteDataAsync(FileName, scooters);
        return NoContent();
    }
}
