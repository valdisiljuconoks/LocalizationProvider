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

            Assert.True(TypeDiscoveryHelper.IsStaticStringProperty(staticField.Item1));
        }

        [Fact]
        public void NestedObject_ScalarProperties_NoAttribute()
        {
            var types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>().ToList();
            var type = types.First();
            var properties = TypeDiscoveryHelper.GetAllProperties(type).ToList();

            Assert.Contains("DbLocalizationProvider.Tests.ResourceKeys.SubResource.AnotherResource", properties.Select(k => k.Item2));
            Assert.Contains("DbLocalizationProvider.Tests.ResourceKeys.SubResource.EvenMoreComplexResource.Amount", properties.Select(k => k.Item2));
        }
    }
}
