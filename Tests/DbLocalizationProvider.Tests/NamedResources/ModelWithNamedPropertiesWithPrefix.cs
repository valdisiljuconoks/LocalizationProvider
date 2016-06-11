namespace DbLocalizationProvider.Tests.NamedResources
{
    [LocalizedModel(KeyPrefix = "/contenttypes/modelwithnamedpropertieswithprefix/")]
    public class ModelWithNamedPropertiesWithPrefix
    {
        [ResourceKey("properties/pageheader/name", Value = "This is page header")]
        public virtual string PageHeader { get; set; }

        [ResourceKey("resource1", Value = "1st resource")]
        [ResourceKey("resource2", Value = "2nd resource")]
        public virtual string JustProperty { get; set; }
    }
}