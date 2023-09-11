using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.NamedResources;

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
