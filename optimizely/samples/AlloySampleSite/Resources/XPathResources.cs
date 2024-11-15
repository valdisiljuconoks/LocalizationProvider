using DbLocalizationProvider.Abstractions;

namespace AlloySampleSite.Resources
{
    [LocalizedResource(KeyPrefix = "/test/xpath/")]
    public class XPathResources
    {
        [ResourceKey("resource1")]
        public static string Resource1{ get; set; } = "Resource 1 from XPath";
    }
}
