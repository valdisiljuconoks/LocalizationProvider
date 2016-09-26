namespace DbLocalizationProvider.Tests.NamedResources
{
    [LocalizedModel]
    public class ModelWithDuplicateResourceKeys
    {
        [ResourceKey("/this/is/key")]
        [ResourceKey("/this/is/key")]
        public virtual string PropertyWithDuplicateKeys { get; set; }
    }
}
