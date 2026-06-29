using System.Collections.Concurrent;
using System.Security.Cryptography;
using MemoryShareCheck.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MemoryShareCheck.Services;

public class AccountService
{
    private readonly ConcurrentDictionary<string, User> _users = new(StringComparer.OrdinalIgnoreCase);

    public User? Authenticate(string username, string password)
    {
        if (!_users.TryGetValue(username, out var user))
            return null;

        return VerifyPassword(password, user.PasswordHash) ? user : null;
    }

    public User? GetById(Guid id)
    {
        return _users.Values.FirstOrDefault(u => u.Id == id);
    }

    public (User? User, string? Error) Register(string username, string email, string password, string? displayName = null, string? phoneNumber = null, string? bio = null)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            DisplayName = displayName,
            PhoneNumber = phoneNumber,
            Bio = bio
        };

        if (!_users.TryAdd(username, user))
            return (null, "Username already exists.");

        return (user, null);
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100_000, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);
        var actualHash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100_000, 32);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
