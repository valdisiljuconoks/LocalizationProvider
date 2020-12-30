using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.HiddenResourcesTests
{
    public class HiddenResourceTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public HiddenResourceTests()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);
        }

        [Fact]
        public void DiscoverHiddenEnumProperties()
        {
            var result = _sut.ScanResources(typeof(SomeEnumWithHiddenResources));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.False(firstResource.IsHidden);

            var middleResource = result.First(r => r.Key == "DbLocalizationProvider.Tests.HiddenResourcesTests.SomeEnumWithHiddenResources.Some");
            Assert.True(middleResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenEnumProperties_WithHiddenAttributeOnClassProperties()
        {
            var result = _sut.ScanResources(typeof(SomeEnumWithAllHiddenResources));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);

            var middleResource = result.First(r => r.Key == "DbLocalizationProvider.Tests.HiddenResourcesTests.SomeEnumWithAllHiddenResources.Some");
            Assert.True(middleResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenModelProperties()
        {
            var result = _sut.ScanResources(typeof(SomeModelWithHiddenProperty));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenModelProperties_WithHiddenAttributeOnClassProperties()
        {
            var result = _sut.ScanResources(typeof(SomeModelWithHiddenPropertyOnClassLevel));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenResourceProperties()
        {
            var result = _sut.ScanResources(typeof(SomeResourcesWithHiddenProperties));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);

            var secondResource = result.First(r => r.Key == "DbLocalizationProvider.Tests.HiddenResourcesTests.SomeResourcesWithHiddenProperties.AnotherProperty");
            Assert.False(secondResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenResources_WithHiddenAttributeOnClassProperties()
        {
            var result = _sut.ScanResources(typeof(SomeResourcesWithHiddenOnClassLevel));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);
        }
    }
}
