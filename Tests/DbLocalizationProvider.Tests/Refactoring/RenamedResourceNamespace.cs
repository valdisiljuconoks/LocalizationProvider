using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedResource]
    [RenamedResource(OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedResourceNamespace
    {
        public static string NewResourceKey => "New Resource Key";
    }
}
