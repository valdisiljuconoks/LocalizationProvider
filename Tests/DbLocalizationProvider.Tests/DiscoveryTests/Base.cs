using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    [LocalizedResource(Inherited = true)]
    public class Base : BadRecursiveResource_BaseDeclaringType { }
}
