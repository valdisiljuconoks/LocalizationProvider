using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private readonly TypeDiscoveryHelper _sut;

        [Fact]
        public async Task ResourceWithBaseClass_NoInheritance_ScannedOnlyDirectProperties()
        {
            var result = await _sut.ScanResources(typeof(ResourceWithBaseClassNoInheritance));

            Assert.Single(result);
        }

        [Fact]
        public async Task ResourceWithBaseClass_ScannedAll()
        {
            var result = await _sut.ScanResources(typeof(ResourceWithBaseClass));

            Assert.Equal(2, result.Count());
        }
    }
}
