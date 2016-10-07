using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    public class ViewModelWithIncludedOnlyTests
    {
        [Fact]
        public void ModelWithBase_IncludedPorperty_ShouldDiscoverOnlyExplicitProperties()
        {
            var properties = TypeDiscoveryHelper.GetAllProperties(typeof(SampleViewModelWithIncludedOnlyWithBase), contextAwareScanning: false)
                                                .Select(p => p.Key)
                                                .ToList();

            Assert.Contains("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.IncludedProperty", properties);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.ExcludedProperty", properties);

            Assert.Contains("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.IncludedBaseProperty", properties);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.ExcludedBaseProperty", properties);
        }

        [Fact]
        public void ModelWithIncludedPorperty_ShouldDiscoverOnlyExplicitProperties()
        {
            var properties = TypeDiscoveryHelper.GetAllProperties(typeof(SampleViewModelWithIncludedOnly), contextAwareScanning: false)
                                                .Select(p => p.Key)
                                                .ToList();

            Assert.Contains("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnly.IncludedProperty", properties);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnly.ExcludedProperty", properties);
        }
    }
}
