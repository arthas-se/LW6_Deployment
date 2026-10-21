using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task2.Logic_Tier.Interfaces;
using Task2.Logic_Tier.Services;
using Task2.Models.Domain;
using Task2.Models.DTO.Post;

namespace Task2.Presentation_Tier.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtTokenGeneratorService _jwtTokenGenerator;
    private readonly IPasswordHasherService _passwordHasher;

    public AuthController(IUserService userService, JwtTokenGeneratorService jwtTokenGenerator, IPasswordHasherService passwordHasher)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserPostDto dto)
    {
        var existingUser = await _userService.GetByLoginAsync(dto.Login);
        if (existingUser != null)
            return BadRequest("User already exists!");

        dto.Password = _passwordHasher.HashPassword(dto.Password);
        await _userService.CreateAsync(dto);

        return Ok("User registered!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserPostDto dto)
    {
        var user = await _userService.GetByLoginAsync(dto.Login);
        if (user == null)
            return Unauthorized("Неправильна пошта або пароль.");

        if (!_passwordHasher.Verify(dto.Password, user.Password))
            return Unauthorized("Неправильна пошта або пароль.");

        var token = _jwtTokenGenerator.Generate(user);
        return Ok(new
        {
            Token = token,
            TokenExpiryTime = DateTime.UtcNow.AddMinutes(60)
        });
    }
}
