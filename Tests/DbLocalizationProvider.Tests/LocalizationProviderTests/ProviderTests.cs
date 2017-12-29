using DbLocalizationProvider.Queries;
using Xunit;

namespace DbLocalizationProvider.Tests.LocalizationProviderTests
{
    public class ProviderTests
    {
        [Fact]
        public void GetNonExistingResource_ReturnsNull()
        {
            ConfigurationContext.Current.CacheManager = new UnitTestCache();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<UnitTestGetTranslationHandler>();

            var result = LocalizationProvider.Current.GetString(() => NonLocalizedResources.SomeProp);

            Assert.Null(result);
        }
    }
}
