// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Storage.PostgreSql
{
    /// <summary>
    /// Gets translation handler
    /// </summary>
    public class GetTranslationHandler : IQueryHandler<GetTranslation.Query, string>
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
            var context = ConfigurationContext.Current;
            if (!context.EnableLocalization()) return query.Key;

            var key = query.Key;

            // we can check whether we know this resource at all
            // if not - we can break circuit here
            if (!ConfigurationContext.Current.BaseCacheManager.KnownResourceKeys.ContainsKey(key)) return null;

            var cacheKey = CacheKeyHelper.BuildKey(key);
            if (context.DiagnosticsEnabled) context.Logger?.Debug($"Executing query for resource key `{query.Key}` (lang: `{query.Language.Name})..");
            var localizationResource = context.CacheManager.Get(cacheKey) as LocalizationResource;

            if (localizationResource == null)
            {
                if (context.DiagnosticsEnabled)
                {
                    context.Logger?.Info(
                        $"MISSING: Resource Key (culture: {query.Language.Name}): {query.Key}. Probably class is not decorated with either [LocalizedModel] or [LocalizedResource] attribute.");
                }

                // resource is not found in the cache, let's check database
                localizationResource = GetResourceFromDb(key) ?? LocalizationResource.CreateNonExisting(key);
                context.CacheManager.Insert(cacheKey, localizationResource, true);
            }

            return localizationResource.Translations.GetValueWithFallback(
                query.Language,
                context.FallbackCultures);
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
