using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedResource]
    [RenamedResource("OldResourceClassAndKeyAndNamespace", OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedResourceClassAndKeyAndNamespace
    {
        [RenamedResource("OldResourceKey")]
        public static string NewResourceKey => "New Resource Key";
    }
}
