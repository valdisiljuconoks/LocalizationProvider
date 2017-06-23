using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    public class ViewModelWithIncludedOnlyTests
    {
        public ViewModelWithIncludedOnlyTests()
        {
            _sut = new TypeDiscoveryHelper();
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        }

        private readonly TypeDiscoveryHelper _sut;

        [Fact]
        public void ModelWithBase_IncludedPorperty_ShouldDiscoverOnlyExplicitProperties()
        {
            var properties = _sut.ScanResources(typeof(SampleViewModelWithIncludedOnlyWithBase))
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
            var properties = _sut.ScanResources(typeof(SampleViewModelWithIncludedOnly))
                                 .Select(p => p.Key)
                                 .ToList();

            Assert.Contains("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnly.IncludedProperty", properties);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnly.ExcludedProperty", properties);
        }
    }
}
