using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring;

[LocalizedResource]
public class RenamedResourceKeyWithAdditionalAttribute
{
    [RenamedResource("OldResourceKey")]
    [AdditionalData]
    public static string NewResourceKey => "New Resource Key";
}
