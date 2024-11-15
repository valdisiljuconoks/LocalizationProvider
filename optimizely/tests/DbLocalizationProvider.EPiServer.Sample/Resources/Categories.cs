using EPiServer.DataAbstraction;

namespace DbLocalizationProvider.EPiServer.Sample.Resources
{
    //[LocalizedResource(KeyPrefix = "/categories/")]
    //public class Categories
    //{
    //    [ResourceKey("category[@name=\"" + nameof(SampleCategory) + "\"]/description")]
    //    public static string SampleCategory => "This is sample cat. from code";
    //}

    [LocalizedCategory]
    public class SampleCategory : Category
    {
        public SampleCategory()
        {
            Name = "This is sample cat. from code";
        }
    }
}
