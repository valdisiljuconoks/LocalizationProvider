using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.DiscoveryTests;

[LocalizedResource(Inherited = true)]
public class BadRecursiveResource_BaseDeclaringType
{
    [Include]
    public static BadRecursiveResource_BaseDeclaringType Instance { get; } = new Base();
}
