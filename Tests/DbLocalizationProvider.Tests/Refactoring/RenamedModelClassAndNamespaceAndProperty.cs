using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource("OldModelClassAndNamespaceAndProperty", OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedModelClassAndNamespaceAndProperty
    {
        [RenamedResource("OldProperty")]
        public string NewProperty { get; set; }
    }
}
