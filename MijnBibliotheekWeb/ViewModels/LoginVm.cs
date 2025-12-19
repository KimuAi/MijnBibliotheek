using System.ComponentModel.DataAnnotations;

namespace MijnBibliotheekWeb.ViewModels;
public class LoginVm
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; } = false;

    public string? ReturnUrl { get; set; }
}
