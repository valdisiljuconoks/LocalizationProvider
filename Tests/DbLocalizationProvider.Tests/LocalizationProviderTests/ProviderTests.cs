using DbLocalizationProvider.Queries;
using Xunit;

namespace DbLocalizationProvider.Tests.LocalizationProviderTests
{
    public class ProviderTests
    {
        [Fact]
        public void GetNonExistingResource_ReturnsNull()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<UnitTestGetTranslationHandler>();

            var result = LocalizationProvider.Current.GetString(() => NonLocalizedResources.SomeProp);

            Assert.Null(result);
        }
    }

    public class NonLocalizedResources
    {
        public static string SomeProp => "Default value for some prop";
    }

    public class UnitTestGetTranslationHandler : GetTranslation.Handler
    {
        protected override LocalizationResource GetResourceFromDb(string key)
        {
            return null;
        }
    }
}
