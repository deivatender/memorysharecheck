using System.ComponentModel.DataAnnotations;

namespace MemoryShareCheck.Models;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
