namespace DbLocalizationProvider.Tests.ForeignKnownResources
{
    public class ResourceWithNoAttribute
    {
        public static string SampleProperty => "Default resource value";

        public class NestedResource
        {
            public static string NestedProperty { get; set; }
        }
    }
}
