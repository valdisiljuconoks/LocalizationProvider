using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace AlloySampleSite.Models.Register;

[LocalizedModel]
public class RegisterViewModel
{
    [Required]
    [Display(Name = "Username")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters a-z, numbers, underscores and hyphens.")]
    [StringLength(20, ErrorMessage = "The {0} field can not be more than {1} characters long.")]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
