using DbLocalizationProvider.AspNet.Queries;

namespace DbLocalizationProvider.Tests.LocalizationProviderTests
{
    public class UnitTestGetTranslationHandler : GetTranslationHandler
    {
        protected override LocalizationResource GetResourceFromDb(string key)
        {
            return null;
        }
    }
}