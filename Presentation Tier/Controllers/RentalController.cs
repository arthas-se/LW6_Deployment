using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO.Patch;
using Task2.Models.DTO.Post;
using Task2.Models.DTO.Business;
using Task2.Services;

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
    public async Task<IActionResult> Create([FromBody] RentalPostDto dto)
    {
        var createdRental = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Create), new { id = createdRental.Id }, createdRental);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Rental updatedRental)
    {
        await _service.UpdateAsync(id, updatedRental);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, [FromBody] RentalPatchDto patchDto)
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

    [HttpPost("start")]
    public async Task<IActionResult> StartRental([FromBody] StartRentalDto dto)
    {
        try
        {
            var rental = await _service.StartRentalAsync(dto);
            return Ok(rental);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("finish")]
    public async Task<IActionResult> FinishRental(FinishRentalDto dto)
    {
        try
        {
            var rental = await _service.FinishRentalAsync(dto);
            return Ok(rental);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelRental([FromBody] CancelRentalDto dto)
    {
        try
        {
            string cancellationReason = "Not specified";
            if (dto != null && dto.CancellationReason != string.Empty)
            {
                cancellationReason = dto.CancellationReason;
            }

            await _service.CancelRentalAsync(dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}