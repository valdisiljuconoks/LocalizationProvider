// Copyright (c) 2017 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore.Queries
{
    public class GetTranslationHandler : GetTranslation.GetTranslationHandlerBase, IQueryHandler<GetTranslation.Query, string>
    {
        public string Execute(GetTranslation.Query query)
        {
            if(!ConfigurationContext.Current.EnableLocalization())
                return query.Key;

            var key = query.Key;
            var language = query.Language;
            var cacheKey = CacheKeyHelper.BuildKey(key);
            var localizationResource = ConfigurationContext.Current.CacheManager.Get(cacheKey) as LocalizationResource;

            if(localizationResource != null)
                return GetTranslationFromAvailableList(localizationResource.Translations, language, query.UseFallback)?.Value;

            var resource = GetResourceFromDb(key);
            LocalizationResourceTranslation localization = null;

            if(resource == null)
                resource = LocalizationResource.CreateNonExisting(key);
            else
                localization = GetTranslationFromAvailableList(resource.Translations, language, query.UseFallback);

            ConfigurationContext.Current.CacheManager.Insert(cacheKey, resource);
            return localization?.Value;
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
