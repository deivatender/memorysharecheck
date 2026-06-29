using MemoryShareCheck.Models;
using MemoryShareCheck.Services;
using Microsoft.AspNetCore.Mvc;

namespace MemoryShareCheck.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    public AuthController(UserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var (user, error) = _userService.Register(request.Username, request.Email, request.Password);
        if (user is null)
            return Conflict(new { message = error });

        var response = _tokenService.GenerateToken(user);
        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _userService.Authenticate(request.Username, request.Password);
        if (user is null)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(_tokenService.GenerateToken(user));
    }
}
