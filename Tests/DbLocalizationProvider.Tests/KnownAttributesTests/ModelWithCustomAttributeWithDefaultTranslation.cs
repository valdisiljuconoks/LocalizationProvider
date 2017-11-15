using System;

namespace DbLocalizationProvider.Tests.KnownAttributesTests
{
    [LocalizedModel]
    public class ModelWithCustomAttributeWithDefaultTranslation
    {
        [AttributeWithDefaultTranslation("This is default translation")]
        public string SomeProperty { get; set; }
    }

    public class AttributeWithDefaultTranslationAttribute : Attribute
    {
        private readonly string _defaultTranslation;

        public AttributeWithDefaultTranslationAttribute(string defaultTranslation)
        {
            _defaultTranslation = defaultTranslation;
        }

        public override string ToString()
        {
            return _defaultTranslation;
        }
    }
}
