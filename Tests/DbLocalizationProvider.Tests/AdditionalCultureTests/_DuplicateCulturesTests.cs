using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.AdditionalCultureTests
{
    public class DuplicateCulturesTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public DuplicateCulturesTests()
        {
            var state = new ScanState();
            var ctx = new ConfigurationContext();
            var keyBuilder = new ResourceKeyBuilder(state, ctx);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<NorwegianDefaultCulture>();
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
        public void DiscoverAdditionalTranslations()
        {
            var results = _sut.ScanResources(typeof(SomeResources));

            Assert.NotEmpty(results);
            Assert.Equal("Navn", results.First().Translations.DefaultTranslation());
            Assert.Equal("Name", results.First().Translations.First(_ => _.Culture == string.Empty).Translation);
        }

        [Fact]
        public void DiscoverAdditionalTranslations_FromEmum()
        {
            var results = _sut.ScanResources(typeof(SomeEnumResource));

            Assert.NotEmpty(results);
            Assert.Equal("Navn", results.First().Translations.DefaultTranslation());
        }
    }
}
