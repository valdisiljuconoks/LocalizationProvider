using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.NamedResources
{
    [LocalizedResource]
    public static class ResourcesWithNamedKeys
    {
        [ResourceKey("/this/is/xpath/to/resource")]
        public static string PageHeader => "This is header";
    }
}
