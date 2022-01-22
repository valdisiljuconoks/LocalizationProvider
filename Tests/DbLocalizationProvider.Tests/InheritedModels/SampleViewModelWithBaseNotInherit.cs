using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.InheritedModels
{
    [LocalizedModel]
    public class VeryBaseLocalizedViewModel
    {
        [Display]
        public string VeryBaseProperty { get; set; }
    }

    [LocalizedModel(Inherited = false)]
    public class BaseLocalizedViewModel : VeryBaseLocalizedViewModel
    {
        [Display]
        [Required]
        public string BaseProperty { get; set; }
    }

    [LocalizedModel(Inherited = false)]
    public class SampleViewModelWithBaseNotInherit : BaseLocalizedViewModel
    {
        public string ChildProperty { get; set; }
    }
}
