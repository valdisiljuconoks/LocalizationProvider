using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.ClassFieldsTests
{
    public class LocalizedResourcesWithFieldsTests
    {
        private readonly ExpressionHelper _expressionHelper;
        private readonly TypeDiscoveryHelper _sut;

        public LocalizedResourcesWithFieldsTests()
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

            _expressionHelper = new ExpressionHelper(keyBuilder);
        }

        [Fact]
        public async Task DiscoverClassField_WithDefaultValue()
        {
            var discoveredResources = await _sut.ScanResources(typeof(LocalizedResourceWithFields));

            // check return
            Assert.NotEmpty(discoveredResources);

            // check discovered translation
            Assert.Equal("sample value", discoveredResources.First().Translations.DefaultTranslation());

            // check generated key from expression
            Assert.Equal("DbLocalizationProvider.Tests.ClassFieldsTests.LocalizedResourceWithFields.ThisisField",
                         _expressionHelper.GetFullMemberName(() => LocalizedResourceWithFields.ThisisField));
        }

        [Fact]
        public async Task DiscoverNoClassField_OnlyWithIgnore()
        {
            var discoveredResources = await _sut.ScanResources(typeof(LocalizedResourceWithIgnoredFields));

            // check return
            Assert.Empty(discoveredResources);
        }
    }
}
