using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring;

[RenamedResource("OldResourceClassWithAdditionalAttribute")]
[LocalizedResource]
public class RenamedResourceClassWithAdditionalAttribute
{
    [AdditionalData]
    public static string NewResourceKey => "New Resource Key";
}
