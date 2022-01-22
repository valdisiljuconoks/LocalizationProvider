using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    [LocalizedModel]
    public class BaseViewModel
    {
        [Display(Name = "Base property", Description = "")]
        [Required]
        public string BaseProperty { get; set; }
    }
}
