using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Controllers;

[Route("api/rental")]
[ApiController]
public class RentalController : ControllerBase
{
    private readonly IRentalService _service;

    public RentalController(IRentalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRentals()
    {
        var rentals = await _service.GetAllAsync();
        return Ok(rentals);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var rental = await _service.GetByIdAsync(id);
        if (rental == null)
            return NotFound("Rental not found!");

        return Ok(rental);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Rental newRental)
    {
        await _service.CreateAsync(newRental);
        return CreatedAtAction(nameof(GetById), new { id = newRental.Id }, newRental);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Rental updatedRental)
    {
        await _service.UpdateAsync(id, updatedRental);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, [FromBody] RentalPatchDTO patchDto)
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

    /*
    [HttpPost("start")]
    public async Task<IActionResult> StartRental([FromBody] StartRentalDto dto)
    {
        // Закоментовано для плавного переходу на сервісний шар та MongoDB
        return StatusCode(501, "Not implemented yet via Service layer");
    }

    [HttpPost("finish/{id}")]
    public async Task<IActionResult> FinishRental(string id)
    {
        return StatusCode(501, "Not implemented yet via Service layer");
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelRental(string id, [FromBody] Dictionary<string, string> body)
    {
        return StatusCode(501, "Not implemented yet via Service layer");
    }
    */
}