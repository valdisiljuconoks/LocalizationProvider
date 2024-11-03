using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.EnumTests;

[LocalizedResource]
public enum SampleEnumWithKeys
{
    None = 0,

    [ResourceKey("/this/is/key")]
    New = 1,
    Open = 2
}
