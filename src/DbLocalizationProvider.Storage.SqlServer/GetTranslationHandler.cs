// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Storage.SqlServer
{
    /// <summary>
    /// Gets translation handler
    /// </summary>
    public class GetTranslationHandler : GetTranslation.GetTranslationHandlerBase, IQueryHandler<GetTranslation.Query, string>
    {
        /// <summary>
        /// Place where query handling happens
        /// </summary>
        /// <param name="query">This is the query instance</param>
        /// <returns>
        /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
        /// will.
        /// </returns>
        public string Execute(GetTranslation.Query query)
        {
            if (!ConfigurationContext.Current.EnableLocalization()) return query.Key;

            var key = query.Key;
            var cacheKey = CacheKeyHelper.BuildKey(key);
            var localizationResource = ConfigurationContext.Current.CacheManager.Get(cacheKey) as LocalizationResource;

            if (localizationResource == null)
            {
                // resource is not found in the cache, let's check database
                localizationResource = GetResourceFromDb(key) ?? LocalizationResource.CreateNonExisting(key);
                ConfigurationContext.Current.CacheManager.Insert(cacheKey, localizationResource, true);
            }

            var fallbackCultures = ConfigurationContext.Current.FallbackCultures;
            return fallbackCultures != null && fallbackCultures.Any()
                ? base.GetTranslationWithFallback(
                    localizationResource.Translations,
                    query.Language,
                    fallbackCultures,
                    query.UseFallback)?.Value
                : base.GetTranslationFromAvailableList(
                    localizationResource.Translations,
                    query.Language,
                    query.UseFallback)?.Value;
        }

        /// <summary>
        /// Gets the resource from database.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected virtual LocalizationResource GetResourceFromDb(string key)
        {
            var q = new GetResource.Query(key);

            return q.Execute();
        }
    }
}
