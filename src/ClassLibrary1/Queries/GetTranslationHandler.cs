using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AspNet.Queries
{
    public class GetTranslationHandler : IQueryHandler<GetTranslation.Query, string>
    {
        public string Execute(GetTranslation.Query query)
        {
            if(!ConfigurationContext.Current.EnableLocalization())
                return query.Key;

            return GetTranslation(query);
        }

        protected virtual string GetTranslation(GetTranslation.Query query)
        {
            var key = query.Key;
            var language = query.Language;
            var cacheKey = CacheKeyHelper.BuildKey(key);
            var localizationResource = ConfigurationContext.Current.CacheManager.Get(cacheKey) as LocalizationResource;

            if(localizationResource != null)
                return GetTranslationFromAvailableList(localizationResource.Translations, language, query.UseFallback)?.Value;

            var resource = GetResourceFromDb(key);
            LocalizationResourceTranslation localization = null;

            // create empty null resource - to indicate non-existing one
            if(resource == null)
                resource = LocalizationResource.CreateNonExisting(key);
            else
                localization = GetTranslationFromAvailableList(resource.Translations, language, query.UseFallback);

            ConfigurationContext.Current.CacheManager.Insert(cacheKey, resource);
            return localization?.Value;
        }

        protected virtual LocalizationResourceTranslation GetTranslationFromAvailableList(
            ICollection<LocalizationResourceTranslation> translations,
            CultureInfo language,
            bool queryUseFallback)
        {
            var foundTranslation = translations?.FirstOrDefault(t => t.Language == language.Name);
            if(foundTranslation == null && queryUseFallback)
                return translations?.FirstOrDefault(t => t.Language == ConfigurationContext.CultureForTranslationsFromCode);

            return foundTranslation;
        }

        protected virtual LocalizationResource GetResourceFromDb(string key)
        {
            using(var db = new LanguageEntities())
            {
                var resource = db.LocalizationResources
                                 .Include(r => r.Translations)
                                 .FirstOrDefault(r => r.ResourceKey == key);

                return resource;
            }
        }
    }
}
