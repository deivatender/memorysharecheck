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

    public TokenService(IConfiguration configuration, RsaSecurityKey signingKey)
    {
        _configuration = configuration;
        _signingKey = signingKey;
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

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration,
            Username = user.Username,
            Email = user.Email
        };
    }
}
