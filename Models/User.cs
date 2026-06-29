namespace MemoryShareCheck.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Gender { get; set; }
    public int? Age { get; set; }
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}
