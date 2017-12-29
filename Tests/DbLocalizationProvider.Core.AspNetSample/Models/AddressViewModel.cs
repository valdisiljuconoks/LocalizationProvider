using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Core.AspNetSample.Models
{
    [LocalizedModel]
    public class AddressViewModel
    {
        [Required]
        public string Street { get; set; }
    }
}
