using Microsoft.AspNetCore.Mvc;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Controllers;

[Route("api/scooter")]
[ApiController]
public class ScooterController : ControllerBase
{
    private readonly IScooterService _service;

    public ScooterController(IScooterService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllScooters()
    {
        var scooters = await _service.GetAllAsync();
        return Ok(scooters);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var scooter = await _service.GetByIdAsync(id);
        if (scooter == null)
            return NotFound("Scooter not found!");

        return Ok(scooter);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Scooter newScooter)
    {
        await _service.CreateAsync(newScooter);
        return CreatedAtAction(nameof(GetById), new { id = newScooter.Id }, newScooter);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Scooter updatedScooter)
    {
        await _service.UpdateAsync(id, updatedScooter);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, [FromBody] ScooterPatchDTO patchDto)
    {
        await _service.UpdatePartialAsync(id, patchDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(string id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}