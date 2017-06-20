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
        public void ThrowOnDuplicateCultures()
        {
            var sut = new TypeDiscoveryHelper();

            Assert.Throws<DuplicateResourceTranslationsException>(() => sut.ScanResources(typeof(SomeResourcesWithDuplicateCultures)));
        }
    }
}
