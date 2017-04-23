using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.UseResourceAttributeTests
{
    [LocalizedModel]
    public class ModelWithOtherResourceUsage
    {
        [UseResource(typeof(CommonResources), nameof(CommonResources.CommonProp))]
        public string SomeProperty { get; set; }
    }
}
