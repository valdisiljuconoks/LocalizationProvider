using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedResource]
    [RenamedResource("OldResourceClass")]
    public class RenamedResourceClass
    {
        public static string NewResourceKey => "New Resource Key";
    }
}
