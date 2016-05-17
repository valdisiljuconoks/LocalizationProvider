using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Tests.NamedResources
{
    [LocalizedModel]
    public class ModelWithNamedProperties
    {
        [ResourceKey("/this/is/xpath/key", Value = "This is page header")]
        [ResourceKey("/this/is/another/xpath/key", Value = "Here could be help text for this property")]
        public virtual string PageHeader { get; set; }

        [ResourceKey("/this/is/complex/type", Value = "Complex type")]
        public virtual ComplexType ComplexProperty { get; set; }

        [Display(Name = "This is simple property")]
        [ResourceKey("/simple/property/with/display/name")]
        public virtual string SimplePropertyWithDisplayName { get; set; }

        public class ComplexType { }
    }

    [LocalizedModel(KeyPrefix = "/contenttypes/modelwithnamedpropertieswithprefix/")]
    public class ModelWithNamedPropertiesWithPrefix
    {
        [ResourceKey("properties/pageheader/name", Value = "This is page header")]
        public virtual string PageHeader { get; set; }

        [ResourceKey("resource1", Value = "1st resource")]
        [ResourceKey("resource2", Value = "2nd resource")]
        public virtual string JustProperty { get; set; }
    }

    [LocalizedModel(KeyPrefix = "/contenttypes/modelwithnamedpropertieswithprefix/")]
    [ResourceKey("/name", Value = "Model with named properties on class")]
    public class ModelWithNamedPropertiesWithPrefixAndKeyOnClass
    {
        [ResourceKey("/name", Value = "This is page header")]
        public virtual string PageHeader { get; set; }
    }
}
