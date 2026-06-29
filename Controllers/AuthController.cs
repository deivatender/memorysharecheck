using MemoryShareCheck.Models;
using MemoryShareCheck.Services;
using Microsoft.AspNetCore.Mvc;

namespace MemoryShareCheck.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly TokenService _tokenService;
    private readonly RefreshTokenService _refreshTokenService;

    public AuthController(AccountService accountService, TokenService tokenService, RefreshTokenService refreshTokenService)
    {
        _accountService = accountService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var (user, error) = _accountService.Register(request.Username, request.Email, request.Password, request.DisplayName);
        if (user is null)
            return Conflict(new { message = error });

        var response = _tokenService.GenerateToken(user);
        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _accountService.Authenticate(request.Username, request.Password);
        if (user is null)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(_tokenService.GenerateToken(user, request.RememberMe ?? false));
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenRequest request)
    {
        var oldToken = _refreshTokenService.Validate(request.RefreshToken);
        if (oldToken is null)
            return Unauthorized(new { message = "Invalid or expired refresh token." });

        var user = _accountService.GetById(oldToken.UserId);
        if (user is null)
            return Unauthorized(new { message = "User not found." });

        _refreshTokenService.Revoke(request.RefreshToken);
        var response = _tokenService.GenerateToken(user);
        return Ok(response);
    }

    [HttpPost("revoke")]
    public IActionResult Revoke([FromBody] RefreshTokenRequest request)
    {
        _refreshTokenService.Revoke(request.RefreshToken);
        return Ok(new { message = "Token revoked." });
    }
}
