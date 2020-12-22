using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.KnownAttributesTests
{
    [LocalizedModel]
    public class ModelWithCustomAttributeWithDefaultTranslation
    {
        [AttributeWithDefaultTranslation("This is default translation")]
        public string SomeProperty { get; set; }
    }
}
