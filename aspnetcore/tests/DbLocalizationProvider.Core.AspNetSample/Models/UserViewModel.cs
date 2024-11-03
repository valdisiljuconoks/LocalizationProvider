using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Core.AspNetSample.Resources;

namespace DbLocalizationProvider.Core.AspNetSample.Models;

[LocalizedModel]
public class UserViewModel
{
    public UserViewModel()
    {
        Address = new AddressViewModel();
    }

    public AddressViewModel Address { get; set; }

    [Display(Name = "User name:", Description = "This is description of UserName field")]
    [Required(ErrorMessage = "Name of the user is required!")]
    [WeirdCustom("Weird UserName attribute")]
    public string UserName { get; set; }

    [Display(Name = "Password:")]
    [Required(ErrorMessage = "Password is kinda required :)")]
    [StringLength(15, MinimumLength = 5, ErrorMessage = "Please use longer password than 5 symbols!!")]
    public string Password { get; set; }

    public string PropertyWithoutDisplayAttribute { get; set; }

    [EmailAddress]
    [Required]
    public string EmailAddress { get; set; }

    [Phone]
    [Required]
    public string Phone { get; set; }

    [Url]
    [Required]
    public string Url { get; set; }

    [Display(Name = "Max length of 5 symbols")]
    [MaxLength(5)]
    [Required]
    public string MaxLength { get; set; }

    [Display(Name = "Min length of 5 symbols")]
    [MinLength(5)]
    [Required]
    public string MinLength { get; set; }

    [Display(Name = "Range between 5 and 10")]
    [Range(5, 10)]
    [Required]
    public string Range { get; set; }

    [Display(Name = "RegExp")]
    [RegularExpression("[a-z]")]
    [Required]
    public string RegularExpression { get; set; }
}
