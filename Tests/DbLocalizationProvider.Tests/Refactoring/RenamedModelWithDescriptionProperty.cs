using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring;

[LocalizedModel]
public class RenamedModelWithDescriptionProperty
{
    [Display(Description = "some text")]
    [RenamedResource("OldProperty")]
    public string NewProperty { get; set; }
}
