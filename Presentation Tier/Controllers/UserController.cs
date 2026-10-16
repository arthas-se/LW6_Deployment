using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task2.Models.Domain;
using Task2.Utils;

namespace Task2.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly JsonFileReader _reader;
    private readonly JsonFileWriter _writer;
    private const string FileName = "users.json";

    public UserController(JsonFileReader reader, JsonFileWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _reader.ReadDataAsync<User>(FileName);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var users = await _reader.ReadDataAsync<User>(FileName);
        var user = users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound("User not found!");

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] User newUser)
    {
        var users = await _reader.ReadDataAsync<User>(FileName);

        newUser.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
        users.Add(newUser);

        await _writer.WriteDataAsync(FileName, users);

        return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] User updatedUser)
    {
        var users = await _reader.ReadDataAsync<User>(FileName);
        var index = users.FindIndex(t => t.Id == id);

        if (index == -1)
            return NotFound($"User with ID={id} not found!");

        updatedUser.Id = id;
        users[index] = updatedUser;

        await _writer.WriteDataAsync(FileName, users);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] Dictionary<string, object> updatedUser)
    {
        var users = await _reader.ReadDataAsync<User>(FileName);
        var user = users.FirstOrDefault(n => n.Id == id);

        if (user == null)
            return NotFound($"User with ID={id} not found!");

        var changes = new Dictionary<string, object>(updatedUser, StringComparer.OrdinalIgnoreCase);

        if (changes.TryGetValue("login", out var loginVal) && loginVal != null)
        {
            user.Login = loginVal.ToString()!;
        }

        if (changes.TryGetValue("balance", out var balanceVal) && decimal.TryParse(balanceVal?.ToString(), out var balance))
        {
            user.Balance = balance;
        }

        if (changes.TryGetValue("blockedDeposit", out var depositVal) && decimal.TryParse(depositVal?.ToString(), out var deposit))
        {
            user.BlockedDeposit = deposit;
        }

        await _writer.WriteDataAsync(FileName, users);

        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(int id)
    {
        var users = await _reader.ReadDataAsync<User>(FileName);
        var index = users.FindIndex(t => t.Id == id);

        if (index == -1)
            return NotFound($"User with ID={id} not found!");

        users.RemoveAt(index);
        await _writer.WriteDataAsync(FileName, users);
        return NoContent();
    }
}
