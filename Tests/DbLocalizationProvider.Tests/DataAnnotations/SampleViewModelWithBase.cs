using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    [LocalizedModel(Inherited = false)]
    public class SampleViewModelWithBase : BaseViewModel
    {
        [Display(Name = "Child property", Description = "")]
        [Required]
        [StringLength(100)]
        public string ChildProperty { get; set; }
    }
}
