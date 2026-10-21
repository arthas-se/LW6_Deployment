using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task2.Logic_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Models.DTO.Patch;
using Task2.Models.DTO.Post;
using Task2.Services;
using Task2.Utils;

namespace Task2.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _service.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _service.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserPostDto dto)
    {
        var createdUser = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Create), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] User updatedUser)
    {
        await _service.UpdateAsync(id, updatedUser);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, [FromBody] UserPatchDto patchDto)
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
