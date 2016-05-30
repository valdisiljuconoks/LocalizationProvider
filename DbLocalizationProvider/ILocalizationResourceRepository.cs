using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider
{
    public interface ILocalizationResourceRepository
    {
        string GetTranslation(string key, CultureInfo language);
        IEnumerable<CultureInfo> GetAvailableLanguages();
        IEnumerable<LocalizationResource> GetAllResources();
        IEnumerable<ResourceItem> GetAllTranslations(string key, CultureInfo language);
        void CreateResource(string key, string username, bool fromCode);
        void DeleteResource(string key);
        void CreateOrUpdateTranslation(string key, CultureInfo language, string newValue);
        void ClearCache();
        LanguageEntities GetDatabaseContext();
    }
}
