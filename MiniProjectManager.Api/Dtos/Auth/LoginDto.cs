using System.ComponentModel.DataAnnotations;

namespace MiniProjectManager.Dtos.Auth;

public class LoginDto
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}