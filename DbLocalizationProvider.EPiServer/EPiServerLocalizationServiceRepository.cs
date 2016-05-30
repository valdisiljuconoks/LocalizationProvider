using System.Collections.Generic;
using System.Globalization;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace DbLocalizationProvider.EPiServer
{
    internal class EPiServerLocalizationServiceRepository : ILocalizationResourceRepository
    {
        private readonly LocalizationService _localizationService;
        private readonly CachedLocalizationResourceRepository _repo;

        public EPiServerLocalizationServiceRepository()
        {
            _localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
            _repo = new CachedLocalizationResourceRepository(new LocalizationResourceRepository(), new EPiServerCacheManager());
        }

        public string GetTranslation(string key, CultureInfo language)
        {
            return _localizationService.GetStringByCulture(key, language);
        }

        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            return _localizationService.AvailableLocalizations;
        }

        public IEnumerable<LocalizationResource> GetAllResources()
        {
            return _repo.GetAllResources();
        }

        public IEnumerable<ResourceItem> GetAllTranslations(string key, CultureInfo language)
        {
            return _repo.GetAllTranslations(key, language);
        }

        public void CreateResource(string key, string username, bool fromCode)
        {
            _repo.CreateResource(key, username, fromCode);
        }

        public void DeleteResource(string key)
        {
            _repo.DeleteResource(key);
        }

        public void CreateOrUpdateTranslation(string key, CultureInfo language, string newValue)
        {
            _repo.CreateOrUpdateTranslation(key, language, newValue);
        }

        public void ClearCache()
        {
            _repo.ClearCache();
        }

        public LanguageEntities GetDatabaseContext()
        {
            return _repo.GetDatabaseContext();
        }
    }
}
