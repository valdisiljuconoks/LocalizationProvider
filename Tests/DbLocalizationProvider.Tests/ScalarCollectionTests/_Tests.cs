using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.ScalarCollectionTests
{
    public class ScalarCollectionTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public ScalarCollectionTests()
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
        public void ScanResourceWillScalarEnumerables_ShouldDiscover()
        {
            var properties = _sut.ScanResources(typeof(ResourceClassWithScalarCollection));

            Assert.Equal(2, properties.Count());
        }

        [Fact]
        public void ScanModelWillScalarEnumerables_ShouldDiscover()
        {
            var properties = _sut.ScanResources(typeof(ModelClassWithScalarCollection));

            Assert.Equal(2, properties.Count());
        }
    }

    [LocalizedResource]
    public class ResourceClassWithScalarCollection
    {
        public int[] ArrayOfItns { get; set; }

        public List<string> CollectionOfStrings { get; set; }
    }

    [LocalizedModel]
    public class ModelClassWithScalarCollection
    {
        public int[] ArrayOfItns { get; set; }

        public List<string> CollectionOfStrings { get; set; }
    }
}
