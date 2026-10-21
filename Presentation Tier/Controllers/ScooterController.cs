using Microsoft.AspNetCore.Mvc;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO.Patch;
using Task2.Models.DTO.Post;
using Task2.Services;

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
    public async Task<IActionResult> Create([FromBody] ScooterPostDto dto)
    {
        var createdScooter = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Create), new { id = createdScooter.Id }, createdScooter);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Scooter updatedScooter)
    {
        await _service.UpdateAsync(id, updatedScooter);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, [FromBody] ScooterPatchDto patchDto)
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