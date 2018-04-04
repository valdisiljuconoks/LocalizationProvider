using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.AdditionalCultureTests
{
    public class DuplicateCulturesTests
    {
        [Fact]
        public void DiscoverAdditionalTranslations()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<NorwegianDefaultCulture>();
            var sut = new TypeDiscoveryHelper();

            var results = sut.ScanResources(typeof(SomeResources));

            Assert.NotEmpty(results);
            Assert.Equal("Navn", results.First().Translations.DefaultTranslation());
            Assert.Equal("Name", results.First().Translations.First(_ => _.Culture == string.Empty).Translation);
        }

        [Fact]
        public void DiscoverAdditionalTranslations_FromEmum()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<NorwegianDefaultCulture>();
            var sut = new TypeDiscoveryHelper();

            var results = sut.ScanResources(typeof(SomeEnumResource));

            Assert.NotEmpty(results);
            Assert.Equal("Navn", results.First().Translations.DefaultTranslation());
        }
    }
}
