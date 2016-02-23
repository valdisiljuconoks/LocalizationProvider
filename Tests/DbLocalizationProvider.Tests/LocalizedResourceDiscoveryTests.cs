using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedResourceDiscoveryTests
    {
        [Fact]
        public void SingleLevel_ScalarProperties_NoAttribute()
        {
            var types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>().ToList();

            Assert.NotEmpty(types);

            var type = types.First();
            var properties = TypeDiscoveryHelper.GetAllProperties(type);

            var staticField = properties.First(p => p.Item2 == "DbLocalizationProvider.Tests.ResourceKeys.ThisIsConstant");

            Assert.True(TypeDiscoveryHelper.IsStringProperty(staticField.Item1.GetGetMethod()));
            Assert.Equal("Default value for constant", staticField.Item3);
        }

        [Fact]
        public void NestedObject_ScalarProperties_NoAttribute()
        {
            var types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>().ToList();
            var type = types.First();
            var properties = TypeDiscoveryHelper.GetAllProperties(type).ToList();

            var complexPropertySubProperty = properties.FirstOrDefault(p => p.Item2 == "DbLocalizationProvider.Tests.ResourceKeys.SubResource.SubResourceProperty");

            Assert.NotNull(complexPropertySubProperty);
            Assert.Equal("Sub Resource Property", complexPropertySubProperty.Item3);

            Assert.Contains("DbLocalizationProvider.Tests.ResourceKeys.SubResource.AnotherResource", properties.Select(k => k.Item2));
            Assert.Contains("DbLocalizationProvider.Tests.ResourceKeys.SubResource.EvenMoreComplexResource.Amount", properties.Select(k => k.Item2));

            // need to check that there is no resource discovered for complex properties itself
            Assert.DoesNotContain("DbLocalizationProvider.Tests.ResourceKeys.SubResource", properties.Select(k => k.Item2));
            Assert.DoesNotContain("DbLocalizationProvider.Tests.ResourceKeys.SubResource.EvenMoreComplexResource", properties.Select(k => k.Item2));

        }
    }
}
