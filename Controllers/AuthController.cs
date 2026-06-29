using MemoryShareCheck.Models;
using MemoryShareCheck.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
        var (user, error) = _accountService.Register(request.Username, request.Email, request.Password, request.DisplayName, request.PhoneNumber, request.Bio, request.DateOfBirth, request.Address, request.ProfilePictureUrl, request.Gender, request.Age, request.Name, request.City, request.State, request.Country);
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

    [Authorize]
    [HttpGet("user-detail")]
    public IActionResult UserDetail()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID claim." });
        }

        var user = _accountService.GetById(userId);
        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.DisplayName,
            user.PhoneNumber,
            user.Bio,
            user.DateOfBirth,
            user.Address,
            user.ProfilePictureUrl,
            user.Gender,
            user.Age,
            user.Name,
            user.City,
            user.State,
            user.Country
        });
    }
}
