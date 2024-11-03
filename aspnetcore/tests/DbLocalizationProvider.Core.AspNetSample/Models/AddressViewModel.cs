using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Core.AspNetSample.Models;

[LocalizedModel]
public class AddressViewModel
{
    [Required(ErrorMessage = "Street name is also required")]
    public string Street { get; set; }
}
