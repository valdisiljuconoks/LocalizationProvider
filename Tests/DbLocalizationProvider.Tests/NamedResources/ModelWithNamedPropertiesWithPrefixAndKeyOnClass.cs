using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.NamedResources;

[LocalizedModel(KeyPrefix = "/contenttypes/modelwithnamedpropertieswithprefixandkeyonclass/")]
[ResourceKey("name", Value = "Model with named properties on class")]
[ResourceKey("description", Value = "This is description of the model")]
public class ModelWithNamedPropertiesWithPrefixAndKeyOnClass
{
    [ResourceKey("properties/pageheader/caption", Value = "This is page header")]
    public virtual string PageHeader { get; set; }
}
