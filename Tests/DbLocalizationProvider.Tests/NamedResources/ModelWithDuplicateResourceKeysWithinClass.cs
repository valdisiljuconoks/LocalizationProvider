using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.NamedResources;

[LocalizedModel]
public class ModelWithDuplicateResourceKeysWithinClass
{
    [ResourceKey("/this/is/key")]
    public virtual string Property1 { get; set; }

    [ResourceKey("/this/is/key")]
    public virtual string Property2 { get; set; }
}
