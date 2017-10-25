using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedResource]
    [RenamedResource("OldResourceClassAndNamespace", OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedResourceClassAndNamespace
    {
        public static string NewResourceKey => "New Resource Key";
    }
}
