// Copyright © 2017 Valdis Iljuconoks.
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

using System;
using DbLocalizationProvider.AspNetCore.Cache;
using DbLocalizationProvider.AspNetCore.DataAnnotations;
using DbLocalizationProvider.AspNetCore.Queries;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDbLocalizationProvider(this IServiceCollection services,
            Action<ConfigurationContext> setup = null)
        {
            // setup default implementations
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>()
                .SetHandler<GetTranslationHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllResources.Query>()
                .SetHandler<GetAllResourcesHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>()
                .SetHandler<DetermineDefaultCulture.Handler>();

            //ConfigurationContext.Current.TypeFactory.ForQuery<AvailableLanguages.Query>().SetHandler<AvailableLanguages.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForQuery<GetAllTranslations.Query>().SetHandler<GetAllTranslations.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<CreateNewResource.Command>().SetHandler<CreateNewResource.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResource.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslation.Handler>();

            ConfigurationContext.Current.TypeFactory.ForCommand<ClearCache.Command>().SetHandler<ClearCacheHandler>();

            var provider = services.BuildServiceProvider();

            // set to default in-memory provider
            var cache = provider.GetService<IMemoryCache>();
            if(cache != null)
                ConfigurationContext.Current.CacheManager = new InMemoryCacheManager(cache);

            // run custom configuration setup (if any)
            setup?.Invoke(ConfigurationContext.Current);

            // setup model metadata providers
            if(ConfigurationContext.Current.ModelMetadataProviders.ReplaceProviders)
                services.Configure<MvcOptions>(_ =>
                {
                    _.ModelMetadataDetailsProviders.Add(new LocalizedDisplayMetadataProvider());
                    _.ModelValidatorProviders.Add(new LocalizedValidationMetadataProvider());
                });

            services.AddDbContext<LanguageEntities>(_ => _.UseSqlServer(ConfigurationContext.Current.Connection));

            return services;
        }
    }
}
