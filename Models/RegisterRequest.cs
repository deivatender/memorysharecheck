using System.ComponentModel.DataAnnotations;

namespace MemoryShareCheck.Models;

public class RegisterRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [StringLength(100)]
    public string? DisplayName { get; set; }

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Url]
    [StringLength(2048)]
    public string? ProfilePictureUrl { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [Range(0, 150)]
    public int? Age { get; set; }

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    public bool? IsLive { get; set; }
}
