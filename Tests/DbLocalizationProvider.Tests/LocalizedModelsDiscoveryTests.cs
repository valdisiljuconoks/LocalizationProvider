using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedModelsDiscoveryTests
    {
        public LocalizedModelsDiscoveryTests()
        {
            var types = new[] { typeof(SampleViewModel), typeof(SubViewModel) };
            var sut = new TypeDiscoveryHelper();

            Assert.NotEmpty(types);

            _properties = types.SelectMany(t => sut.ScanResources(t));
        }

        private readonly IEnumerable<DiscoveredResource> _properties;

        [Fact]
        public void PropertyWithAttributes_DisplayDescription_Discovered()
        {
            var resource = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.PropertyWithDescription");
            Assert.NotNull(resource);

            var propertyWithDescriptionResource = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.PropertyWithDescription-Description");
            Assert.NotNull(propertyWithDescriptionResource);
        }

        [Fact]
        public void SingleLevel_ScalarProperties_NoAttribute()
        {
            var simpleProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty");
            Assert.NotNull(simpleProperty);

            var ignoredProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.IgnoredProperty");
            Assert.Null(ignoredProperty);

            Assert.Equal("SampleProperty", simpleProperty.Translation);

            var simplePropertyWithDefaultValue = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty2");
            Assert.NotNull(simplePropertyWithDefaultValue);
            Assert.Equal("This is Display value", simplePropertyWithDefaultValue.Translation);

            var simplePropertyRequired = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-Required");
            Assert.NotNull(simplePropertyRequired);
            Assert.Equal("SampleProperty-Required", simplePropertyRequired.Translation);

            var simplePropertyStringLength = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-StringLength");
            Assert.NotNull(simplePropertyStringLength);

            var subProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SubViewModel.AnotherProperty");
            Assert.NotNull(subProperty);

            var includedSubProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.ComplexIncludedProperty");
            Assert.NotNull(includedSubProperty);

            var nullable = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.NullableInt");
            Assert.NotNull(nullable);
        }
    }
}
