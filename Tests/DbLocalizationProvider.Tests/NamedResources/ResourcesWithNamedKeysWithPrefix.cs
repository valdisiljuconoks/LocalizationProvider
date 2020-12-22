using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.NamedResources
{
    [LocalizedResource(KeyPrefix = "/this/is/root/resource/")]
    public static class ResourcesWithNamedKeysWithPrefix
    {
        [ResourceKey("and/this/is/header")]
        public static string PageHeader => "This is header";

        [ResourceKey("and/1stresource", Value = "Value in attribute")]
        [ResourceKey("and/2ndresource")]
        public static string SomeResource => "This is property value";
    }
}
