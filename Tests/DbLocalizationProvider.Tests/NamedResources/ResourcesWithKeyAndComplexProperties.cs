using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.NamedResources;

[LocalizedResource(KeyPrefix = "Prefix")]
public static class ResourcesWithKeyAndComplexProperties
{
    public static ComplexNestedClass NestedProperty { get; set; }

    public class ComplexNestedClass
    {
        public string SomeProperty { get; set; }
    }
}
