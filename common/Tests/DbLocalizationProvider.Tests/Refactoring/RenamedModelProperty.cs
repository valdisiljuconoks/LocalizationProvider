using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring;

[LocalizedModel]
public class RenamedModelProperty
{
    [RenamedResource("OldProperty")]
    public string NewProperty { get; set; }
}
