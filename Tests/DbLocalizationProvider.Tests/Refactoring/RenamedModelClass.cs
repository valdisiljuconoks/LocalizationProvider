using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource("OldModelClass")]
    public class RenamedModelClass
    {
        public string NewProperty { get; set; }
    }
}
