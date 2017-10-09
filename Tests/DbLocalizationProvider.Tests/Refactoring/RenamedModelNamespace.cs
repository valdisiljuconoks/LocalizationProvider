using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource(OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedModelNamespace
    {
        public string NewProperty { get; set; }
    }
}
