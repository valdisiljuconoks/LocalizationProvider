using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Tests.InheritedModels
{
    public class BaseViewModel
    {
        [Display]
        [Required]
        public string BaseProperty { get; set; }
    }
}
