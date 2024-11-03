using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.EnumTests;

[LocalizedResource(KeyPrefix = "/this/is/prefix")]
public enum SampleEnumWithKeysWithClassPrefix
{
    None = 0,

    [ResourceKey("/and/this/is/key")]
    New = 1,
    Open = 2
}
