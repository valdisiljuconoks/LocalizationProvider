using System.ComponentModel.DataAnnotations;

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
