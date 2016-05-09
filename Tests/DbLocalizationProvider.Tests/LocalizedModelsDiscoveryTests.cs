using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedModelsDiscoveryTests
    {
        [Fact]
        public void SingleLevel_ScalarProperties_NoAttribute()
        {
            var types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedModelAttribute>().ToList();

            Assert.NotEmpty(types);

            var properties = types.SelectMany(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false));

            var simpleProperty = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty");
            Assert.NotNull(simpleProperty);

            var ignoredProperty = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.IgnoredProperty");
            Assert.Null(ignoredProperty);

            Assert.Equal("SampleProperty", simpleProperty.Translation);

            var simplePropertyWithDefaultValue = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty2");
            Assert.NotNull(simplePropertyWithDefaultValue);
            Assert.Equal("This is Display value", simplePropertyWithDefaultValue.Translation);

            var simplePropertyRequired = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-Required");
            Assert.NotNull(simplePropertyRequired);
            Assert.Equal("SampleProperty-Required", simplePropertyRequired.Translation);

            var simplePropertyStringLength = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-StringLength");
            Assert.NotNull(simplePropertyStringLength);

            var subProperty = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SubViewModel.AnotherProperty");
            Assert.NotNull(subProperty);

            var includedSubProperty = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.ComplexIncludedProperty");
            Assert.NotNull(includedSubProperty);

            var nonExistingEmailDataTypeResource = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.Email-DataTypeEmailAddress");
            Assert.NotNull(nonExistingEmailDataTypeResource);

            var nullable = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.NullableInt");
            Assert.NotNull(nullable);
        }
    }
}
