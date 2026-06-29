using System.Collections.Concurrent;
using System.Security.Cryptography;
using MemoryShareCheck.Models;

namespace MemoryShareCheck.Services;

public class RefreshTokenService
{
    private readonly ConcurrentDictionary<string, RefreshToken> _refreshTokens = new();
    private readonly IConfiguration _configuration;

    public RefreshTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        var expirationDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays", 7);
        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var tokenString = Convert.ToBase64String(tokenBytes);

        var refreshToken = new RefreshToken
        {
            Token = tokenString,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays)
        };

        _refreshTokens[tokenString] = refreshToken;
        return refreshToken;
    }

    public RefreshToken? Validate(string token)
    {
        if (!_refreshTokens.TryGetValue(token, out var refreshToken))
            return null;

        if (refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            _refreshTokens.TryRemove(token, out _);
            return null;
        }

        return refreshToken;
    }

    public RefreshToken? RotateRefreshToken(string oldToken)
    {
        var existing = Validate(oldToken);
        if (existing is null)
            return null;

        existing.IsRevoked = true;
        _refreshTokens.TryRemove(oldToken, out _);

        return GenerateRefreshToken(existing.UserId);
    }

    public void Revoke(string token)
    {
        if (_refreshTokens.TryGetValue(token, out var refreshToken))
        {
            refreshToken.IsRevoked = true;
            _refreshTokens.TryRemove(token, out _);
        }
    }

    public void RevokeAllForUser(Guid userId)
    {
        var userTokens = _refreshTokens.Where(kvp => kvp.Value.UserId == userId).ToList();
        foreach (var kvp in userTokens)
        {
            kvp.Value.IsRevoked = true;
            _refreshTokens.TryRemove(kvp.Key, out _);
        }
    }
}
