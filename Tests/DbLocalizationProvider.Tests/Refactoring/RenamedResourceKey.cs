using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedResource]
    public class RenamedResourceKey
    {
        [RenamedResource("OldResourceKey")]
        public static string NewResourceKey => "New Resource Key";
    }
}
