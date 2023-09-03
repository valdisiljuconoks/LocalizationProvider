using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    public class ViewModelWithIncludedOnlyTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public ViewModelWithIncludedOnlyTests()
        {
            var state = new ScanState();
            var ctx = new ConfigurationContext();
            var keyBuilder = new ResourceKeyBuilder(state, ctx);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx.TypeFactory);
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
        public async Task ModelWithBase_IncludedProperty_ShouldDiscoverOnlyExplicitProperties()
        {
            var properties = (await _sut.ScanResources(typeof(SampleViewModelWithIncludedOnlyWithBase)))
                .Select(p => p.Key)
                .ToList();

            Assert.Contains("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.IncludedProperty", properties);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.ExcludedProperty", properties);

            Assert.Contains("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.IncludedBaseProperty", properties);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.ExcludedBaseProperty", properties);
        }

        [Fact]
        public async Task ModelWithIncludedProperty_ShouldDiscoverOnlyExplicitProperties()
        {
            var properties = (await _sut.ScanResources(typeof(SampleViewModelWithIncludedOnly)))
                .Select(p => p.Key)
                .ToList();

            Assert.Contains("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnly.IncludedProperty", properties);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnly.ExcludedProperty", properties);
        }
    }
}
