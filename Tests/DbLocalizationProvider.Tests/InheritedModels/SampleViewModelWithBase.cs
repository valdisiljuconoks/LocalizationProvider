using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.InheritedModels;

public class BaseViewModel
{
    [Display]
    [Required]
    public string BaseProperty { get; set; }
}

[LocalizedModel]
public class SampleViewModelWithBase : BaseViewModel
{
    public string ChildProperty { get; set; }
}
