using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.NamedResources
{
    [LocalizedResource]
    public class BadResourceWithDuplicateKeysWithinClass
    {
        [ResourceKey("/this/is/key")]
        public static string Property1 { get; set; }

        [ResourceKey("/this/is/key")]
        public static string Property2 { get; set; }
    }
}
