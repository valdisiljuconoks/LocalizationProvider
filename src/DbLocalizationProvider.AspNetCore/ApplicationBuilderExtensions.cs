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
using DbLocalizationProvider.AspNetCore.Sync;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore
{

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDbLocalizationProvider(this IServiceCollection services, Action<ConfigurationContext> setup = null)
        {
            setup?.Invoke(ConfigurationContext.Current);

            services.AddDbContext<LanguageEntities>(_ => _.UseSqlServer(ConfigurationContext.Current.ConnectionName));

            return services;
        }
    }

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDbLocalizationProvider(this IApplicationBuilder builder, Action<ConfigurationContext> setup = null)
        {
            // create db schema
            var ctx = new LanguageEntities();
            ctx.Database.Migrate();

            var synchronizer = new ResourceSynchronizer();
            synchronizer.DiscoverAndRegister();



            // setup default implementations
            //ConfigurationContext.Current.TypeFactory.ForQuery<AvailableLanguages.Query>().SetHandler<AvailableLanguages.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslation.Handler>();

            //ConfigurationContext.Current.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResources.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForQuery<GetAllTranslations.Query>().SetHandler<GetAllTranslations.Handler>();

            //ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            //ConfigurationContext.Current.TypeFactory.ForCommand<CreateNewResource.Command>().SetHandler<CreateNewResource.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResource.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslation.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<ClearCache.Command>().SetHandler<ClearCache.Handler>();

            if(setup != null)
                ConfigurationContext.Setup(setup);

            var distCache = builder.ApplicationServices.GetService<IDistributedCache>();

            var memCache = builder.ApplicationServices.GetService<IMemoryCache>();
            ConfigurationContext.Current.CacheManager = new InMemoryCacheManager(memCache);

            //var synchronizer = new ResourceSynchronizer();
            //synchronizer.DiscoverAndRegister();

            // set model metadata providers
            //if(ConfigurationContext.Current.ModelMetadataProviders.ReplaceProviders)
            //{
            //    // set current provider
            //    if(ModelMetadataProviders.Current == null)
            //    {
            //        if(ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
            //        {
            //            ModelMetadataProviders.Current = new CachedLocalizedMetadataProvider();
            //        }
            //        else
            //        {
            //            ModelMetadataProviders.Current = new LocalizedMetadataProvider();
            //        }
            //    }
            //    else
            //    {
            //        if (ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
            //        {
            //            ModelMetadataProviders.Current = new CompositeModelMetadataProvider<CachedLocalizedMetadataProvider>(ModelMetadataProviders.Current);
            //        }
            //        else
            //        {
            //            ModelMetadataProviders.Current = new CompositeModelMetadataProvider<LocalizedMetadataProvider>(ModelMetadataProviders.Current);
            //        }
            //    }

            //    for (var i = 0; i < ModelValidatorProviders.Providers.Count; i++)
            //    {
            //        var provider = ModelValidatorProviders.Providers[i];
            //        if (!(provider is DataAnnotationsModelValidatorProvider))
            //        {
            //            continue;
            //        }

            //        ModelValidatorProviders.Providers.RemoveAt(i);
            //        ModelValidatorProviders.Providers.Insert(i, new LocalizedModelValidatorProvider());
            //        break;
            //    }
            //}

            return builder;
        }
    }
}
