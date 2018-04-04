using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.AdditionalCultureTests
{
    public class AdditionalCulturesTests
    {
        public AdditionalCulturesTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        }

        [Fact]
        public void DiscoverAdditionalTranslations()
        {
            var sut = new TypeDiscoveryHelper();

            var results = sut.ScanResources(typeof(SomeResources));

            Assert.NotEmpty(results);
            Assert.Equal("Name", results.First().Translations.DefaultTranslation());
        }

        [Fact]
        public void DiscoverAdditionalTranslations_ForResourceWithKeys()
        {
            var sut = new TypeDiscoveryHelper();

            var results = sut.ScanResources(typeof(SomeResourcesWithKeys));

            Assert.NotEmpty(results);
            Assert.Equal("Noen i norsk", results.First().Translations.First(t => t.Culture == "no").Translation);
        }

        [Fact]
        public void DiscoverAdditionalTranslations_FromEmum()
        {
            var sut = new TypeDiscoveryHelper();

            var results = sut.ScanResources(typeof(SomeEnumResource));

            Assert.NotEmpty(results);
            Assert.Equal("Name", results.First().Translations.DefaultTranslation());
        }

        [Fact]
        public void ThrowOnDuplicateCultures_FromEnum()
        {
            var sut = new TypeDiscoveryHelper();

            Assert.Throws<DuplicateResourceTranslationsException>(() => sut.ScanResources(typeof(SomeEnumResourceWithDuplicateCultures)));
        }

        [Fact]
        public void ThrowOnDuplicateCultures_FromOrdinaryResource()
        {
            var sut = new TypeDiscoveryHelper();

            Assert.Throws<DuplicateResourceTranslationsException>(() => sut.ScanResources(typeof(SomeResourcesWithDuplicateCultures)));
        }
    }
}
