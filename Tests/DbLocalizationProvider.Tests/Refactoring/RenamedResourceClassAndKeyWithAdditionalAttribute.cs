using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring {
    [RenamedResource("OldResourceClassAndKeyWithAdditionalAttribute")]
    [LocalizedResource]
    public class RenamedResourceClassAndKeyWithAdditionalAttribute
    {
        [RenamedResource("OldResourceKey")]
        [AdditionalData]
        public static string NewResourceKey => "New Resource Key";
    }
}