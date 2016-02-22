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

            var simpleProperty = properties.FirstOrDefault(p => p.Item2 == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty");
            Assert.NotNull(simpleProperty);
            Assert.Equal("DbLocalizationProvider.Tests.SampleViewModel.SampleProperty", simpleProperty.Item3);

            var simplePropertyWithDefaultValue = properties.FirstOrDefault(p => p.Item2 == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty2");
            Assert.NotNull(simplePropertyWithDefaultValue);
            Assert.Equal("This is Display value", simplePropertyWithDefaultValue.Item3);

            var simplePropertyRequired = properties.FirstOrDefault(p => p.Item2 == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-Required");
            Assert.NotNull(simplePropertyRequired);
            Assert.Equal("DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-Required", simplePropertyRequired.Item3);

            var simplePropertyStringLength = properties.FirstOrDefault(p => p.Item2 == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-StringLength");
            Assert.NotNull(simplePropertyStringLength);

            var subProperty = properties.FirstOrDefault(p => p.Item2 == "DbLocalizationProvider.Tests.SubViewModel.AnotherProperty");
            Assert.NotNull(subProperty);
        }
    }
}
