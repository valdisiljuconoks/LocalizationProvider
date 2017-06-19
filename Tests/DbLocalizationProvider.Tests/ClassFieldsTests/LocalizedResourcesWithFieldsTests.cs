using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.ClassFieldsTests
{
    public class LocalizedResourcesWithFieldsTests
    {
        public LocalizedResourcesWithFieldsTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        }

        [Fact]
        public void DiscoverClassField_WithDefaultValue()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredResources = sut.ScanResources(typeof(LocalizedResourceWithFields));

            // check return
            Assert.NotEmpty(discoveredResources);

            // check discovered translation
            Assert.Equal("sample value", discoveredResources.First().Translations.DefaultTranslation());

            // check generated key from expression
            Assert.Equal("DbLocalizationProvider.Tests.ClassFieldsTests.LocalizedResourceWithFields.ThisisField",
                         ExpressionHelper.GetFullMemberName(() => LocalizedResourceWithFields.ThisisField));
        }

        [Fact]
        public void DiscoverNoClassField_OnlyWithIgnore()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredResources = sut.ScanResources(typeof(LocalizedResourceWithIgnoredFields));

            // check return
            Assert.Empty(discoveredResources);
        }
    }
}
