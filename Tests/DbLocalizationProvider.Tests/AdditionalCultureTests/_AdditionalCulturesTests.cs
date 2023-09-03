using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.AdditionalCultureTests
{
    public class AdditionalCulturesTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public AdditionalCulturesTests()
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
        public async Task DiscoverAdditionalTranslations()
        {
            var results = await _sut.ScanResources(typeof(SomeResources));

            Assert.NotEmpty(results);
            Assert.Equal("Name", results.First().Translations.DefaultTranslation());
        }

        [Fact]
        public async Task DiscoverAdditionalTranslations_ForResourceWithKeys()
        {
            var results = await _sut.ScanResources(typeof(SomeResourcesWithKeys));

            Assert.NotEmpty(results);
            Assert.Equal("Noen i norsk", results.First().Translations.First(t => t.Culture == "no").Translation);
        }

        [Fact]
        public async Task DiscoverAdditionalTranslations_FromEmum()
        {
            var results = await _sut.ScanResources(typeof(SomeEnumResource));

            Assert.NotEmpty(results);
            Assert.Equal("Name", results.First().Translations.DefaultTranslation());
        }

        [Fact]
        public void ThrowOnDuplicateCultures_FromEnum()
        {
            Assert.ThrowsAsync<DuplicateResourceTranslationsException>(async () => await _sut.ScanResources(typeof(SomeEnumResourceWithDuplicateCultures)));
        }

        [Fact]
        public void ThrowOnDuplicateCultures_FromOrdinaryResource()
        {
            Assert.ThrowsAsync<DuplicateResourceTranslationsException>(async () => await _sut.ScanResources(typeof(SomeResourcesWithDuplicateCultures)));
        }

        [Fact]
        public void ScanResource_BadTranslationLanguage()
        {
            Assert.ThrowsAsync<CultureNotFoundException>(async () => await _sut.ScanResources(typeof(BadResourceWithNoExistingLanguageCode)));
        }
    }
}
