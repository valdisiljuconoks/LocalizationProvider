using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Core.AspNetSample.Models.AccountViewModels;

public class ExternalLoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
