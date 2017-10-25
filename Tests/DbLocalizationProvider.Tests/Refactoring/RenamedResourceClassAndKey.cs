using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring {
    [LocalizedResource]
    [RenamedResource("OldResourceClass")]
    public class RenamedResourceClassAndKey
    {
        [RenamedResource("OldResourceKey")]
        public static string NewResourceKey => "New Resource Key";
    }
}