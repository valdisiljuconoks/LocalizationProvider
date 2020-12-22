using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.ResourcesAndInheritance
{
    public class InheritanceTests
    {
        public InheritanceTests()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedEnumTypeScanner(keyBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state)
            });

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
