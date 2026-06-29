using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using MemoryShareCheck.Models;
using Microsoft.IdentityModel.Tokens;

namespace MemoryShareCheck.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly RsaSecurityKey _signingKey;
    private readonly RefreshTokenService _refreshTokenService;

    public TokenService(IConfiguration configuration, RsaSecurityKey signingKey, RefreshTokenService refreshTokenService)
    {
        _configuration = configuration;
        _signingKey = signingKey;
        _refreshTokenService = refreshTokenService;
    }

    public AuthResponse GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256);
        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);
        var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        var refreshToken = _refreshTokenService.GenerateRefreshToken(user.Id);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresAt,
            Username = user.Username,
            Email = user.Email
        };
    }
}
