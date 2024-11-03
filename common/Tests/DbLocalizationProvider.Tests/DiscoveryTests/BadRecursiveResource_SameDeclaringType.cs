using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.DiscoveryTests;

[LocalizedResource]
public class BadRecursiveResource_SameDeclaringType
{
    [Include]
    public static BadRecursiveResource_SameDeclaringType Instance { get; } = new();
}
