using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Optimizely.Sandbox.Models;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
