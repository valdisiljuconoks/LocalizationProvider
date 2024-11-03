using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring;

[LocalizedModel]
[RenamedResource("OldModelClassAndNamespace", OldNamespace = "In.Galaxy.Far.Far.Away")]
public class RenamedModelClassAndNamespace
{
    public string NewProperty { get; set; }
}
