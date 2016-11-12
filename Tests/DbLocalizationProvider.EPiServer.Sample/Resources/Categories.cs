namespace DbLocalizationProvider.EPiServer.Sample.Resources
{
    [LocalizedResource(KeyPrefix = "/categories/")]
    public class Categories
    {
        [ResourceKey("category[@name=\"" + nameof(SampleCategory) + "\"]/description")]
        public static string SampleCategory => "This is sample cat. from code";
    }
}
