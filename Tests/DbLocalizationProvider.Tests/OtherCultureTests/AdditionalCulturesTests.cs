using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.OtherCultureTests
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

            //var results = sut.ScanResources(typeof(SomeResources));

            Assert.True(false);
        }
    }

    [LocalizedResource]
    public class SomeResources
    {
        [TranslationForCulture("Navn", "no")]
        public static string SomeProperty => "Name";
    }
}
