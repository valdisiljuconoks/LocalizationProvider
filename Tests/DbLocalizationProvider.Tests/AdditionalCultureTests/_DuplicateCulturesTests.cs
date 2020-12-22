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
        private TypeDiscoveryHelper _sut;

        public DuplicateCulturesTests()
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
        }

        [Fact]
        public void DiscoverAdditionalTranslations()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<NorwegianDefaultCulture>();
            var results = _sut.ScanResources(typeof(SomeResources));

            Assert.NotEmpty(results);
            Assert.Equal("Navn", results.First().Translations.DefaultTranslation());
            Assert.Equal("Name", results.First().Translations.First(_ => _.Culture == string.Empty).Translation);
        }

        [Fact]
        public void DiscoverAdditionalTranslations_FromEmum()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<NorwegianDefaultCulture>();

            var results = _sut.ScanResources(typeof(SomeEnumResource));

            Assert.NotEmpty(results);
            Assert.Equal("Navn", results.First().Translations.DefaultTranslation());
        }
    }
}
