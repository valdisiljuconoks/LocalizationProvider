using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Tests.InheritedModels
{
    [LocalizedModel]
    public class BaseOpenViewModel<T>
    {
        [Display]
        [Required]
        public string BaseProperty { get; set; }
    }

    [LocalizedModel(Inherited = false)]
    public class SampleViewModelWithClosedBase : BaseOpenViewModel<SomeType>
    {
        public string ChildProperty { get; set; }
    }

    public class SomeType { }
}
