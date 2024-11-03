using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.NamedResources;

[LocalizedResource]
public class BadResourceWithDuplicateKeys
{
    [ResourceKey("/this/is/key")]
    [ResourceKey("/this/is/key")]
    public static string PropertyWithDuplicateKeys { get; set; }
}
