//// Copyright (c) Valdis Iljuconoks. All rights reserved.
//// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

//using System.Collections.Generic;
//using System.Globalization;
//using DbLocalizationProvider.Abstractions;
//using DbLocalizationProvider.Cache;
//using DbLocalizationProvider.Queries;

//namespace DbLocalizationProvider.Storage.SqlServer
//{
//    /// <summary>
//    /// Reads all available languages form database (in which translations are added).
//    /// </summary>
//    public class AvailableLanguagesHandler : IQueryHandler<AvailableLanguages.Query, IEnumerable<CultureInfo>>
//    {
//        private readonly ConfigurationContext _configurationContext;

//        /// <summary>
//        /// Creates new instance of the handler.
//        /// </summary>
//        /// <param name="configurationContext">Configuration settings.</param>
//        public AvailableLanguagesHandler(ConfigurationContext configurationContext)
//        {
//            _configurationContext = configurationContext;
//        }

//        /// <summary>
//        /// Place where query handling happens
//        /// </summary>
//        /// <param name="query">This is the query instance</param>
//        /// <returns>
//        /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
//        /// will.
//        /// </returns>
//        public IEnumerable<CultureInfo> Execute(AvailableLanguages.Query query)
//        {
//            var cacheKey = CacheKeyHelper.BuildKey($"AvailableLanguages_{query.IncludeInvariant}");
//            if (_configurationContext.CacheManager.Get(cacheKey) is IEnumerable<CultureInfo> cachedLanguages)
//            {
//                return cachedLanguages;
//            }

//            var languages = GetAvailableLanguages(query.IncludeInvariant);
//            _configurationContext.CacheManager.Insert(cacheKey, languages, false);

//            return languages;
//        }

//        private IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant)
//        {
//            var repo = new ResourceRepository(_configurationContext);

//            return repo.GetAvailableLanguages(includeInvariant);
//        }
//    }
//}
