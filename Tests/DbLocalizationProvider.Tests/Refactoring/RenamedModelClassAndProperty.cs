using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource("OldModelClassAndProperty")]
    public class RenamedModelClassAndProperty
    {
        [RenamedResource("OldProperty")]
        public string NewProperty { get; set; }
    }
}
