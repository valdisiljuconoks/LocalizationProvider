using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedResourceDiscoveryTests
    {
        private List<Type> _types;

        public LocalizedResourceDiscoveryTests()
        {
            _types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>().ToList();
            Assert.NotEmpty(_types);
        }

        [Fact]
        public void SingleLevel_ScalarProperties()
        {
            var type = _types.First(t => t.FullName == "DbLocalizationProvider.Tests.ResourceKeys");
            var properties = TypeDiscoveryHelper.GetAllProperties(type);

            var staticField = properties.First(p => p.Item2 == "DbLocalizationProvider.Tests.ResourceKeys.ThisIsConstant");

            Assert.True(TypeDiscoveryHelper.IsStringProperty(staticField.Item1.GetGetMethod()));
            Assert.Equal("Default value for constant", staticField.Item3);
        }

        [Fact]
        public void NestedObject_ScalarProperties()
        {
            var type = _types.First(t => t.FullName == "DbLocalizationProvider.Tests.ResourceKeys");
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

        [Fact]
        public void NestedType_ScalarProperties()
        {
            var type = _types.First(t => t.FullName == "DbLocalizationProvider.Tests.PageResources");

            Assert.NotNull(type);

            var property = TypeDiscoveryHelper.GetAllProperties(type).FirstOrDefault(p => p.Item2 == "DbLocalizationProvider.Tests.PageResources.Header.HelloMessage");

            Assert.NotNull(property);
        }
    }
}
