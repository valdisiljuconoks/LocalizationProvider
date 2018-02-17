using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.ResourcesAndInheritance
{
    public class InheritanceTests
    {
        public InheritanceTests()
        {
            _sut = new TypeDiscoveryHelper();
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        }

        private readonly TypeDiscoveryHelper _sut;

        [Fact]
        public void ResourceWithBaseClass_NoInheritance_ScannedOnlyDirectProperties()
        {
            var result = _sut.ScanResources(typeof(ResourceWithBaseClassNoInheritance));

            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void ResourceWithBaseClass_ScannedAll()
        {
            var result = _sut.ScanResources(typeof(ResourceWithBaseClass));

            Assert.Equal(2, result.Count());
        }
    }
}
