using Microsoft.AspNetCore.Mvc;
using Task2.Logic_Tier.Interfaces;
using Task2.Logic_Tier.Services;
using Task2.Models.Domain;
using Task2.Models.DTO.Jwt;
using Task2.Models.DTO.Post;

namespace Task2.Presentation_Tier.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGeneratorService _jwtTokenGenerator;
    private readonly IPasswordHasherService _passwordHasher;

    public AuthController(
        IUserService userService,
        IJwtTokenGeneratorService jwtTokenGenerator,
        IPasswordHasherService passwordHasher)
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
        if (user == null || !_passwordHasher.Verify(dto.Password, user.Password))
            return Unauthorized("Invalid login or password.");
        
        var accessToken = _jwtTokenGenerator.Generate(user);

        var refreshToken = Guid.NewGuid().ToString();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);

        await _userService.UpdateAsync(user.Id, user);

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenExpiryTime = DateTime.UtcNow.AddMinutes(60)
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        if (string.IsNullOrEmpty(dto?.RefreshToken))
            return BadRequest("Refresh token is required.");

        var user = await _userService.GetByRefreshTokenAsync(dto.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Unauthorized("Недійсний або прострочений рефреш-токен.");
        
        var newAccessToken = _jwtTokenGenerator.Generate(user);
        var newRefreshToken = Guid.NewGuid().ToString();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userService.UpdateAsync(user.Id, user);

        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}