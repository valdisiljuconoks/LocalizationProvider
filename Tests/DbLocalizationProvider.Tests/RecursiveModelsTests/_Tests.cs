using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.RecursiveModelsTests
{
    public class Tests
    {
        private readonly TypeDiscoveryHelper _sut;

        public Tests()
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
        public void Test1()
        {
            _sut.ScanResources(typeof(ResourceClassWithRecursiveProperty));

            Assert.True(TypeDiscoveryHelper.DiscoveredResourceCache.ContainsKey("DbLocalizationProvider.Tests.RecursiveModelsTests.ResourceClassWithRecursiveProperty"));
        }
    }

    [LocalizedResource]
    public class ResourceClassWithRecursiveProperty
    {
        public ResourceClassWithRecursiveProperty NestedClass { get; set; }
    }

}
