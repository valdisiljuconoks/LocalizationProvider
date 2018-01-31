// Copyright(c) 2018 Valdis Iljuconoks.
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
using System.Configuration;
using System.Web.Mvc;
using DbLocalizationProvider.AspNet.Cache;
using DbLocalizationProvider.AspNet.Commands;
using DbLocalizationProvider.AspNet.Queries;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.DataAnnotations;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Owin;

namespace DbLocalizationProvider
{
    public static class IAppBuilderExtensions
    {
        public static IAppBuilder UseDbLocalizationProvider(this IAppBuilder builder, Action<ConfigurationContext> setup = null)
        {
            // setup default implementations
            ConfigurationContext.Current.TypeFactory.ForQuery<AvailableLanguages.Query>().SetHandler<AvailableLanguagesHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslationHandler>();

            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResourcesHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllTranslations.Query>().SetHandler<GetAllTranslationsHandler>();

            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            ConfigurationContext.Current.TypeFactory.ForCommand<CreateNewResource.Command>().SetHandler<CreateNewResourceHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResourceHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslationHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<ClearCache.Command>().SetHandler<ClearCacheHandler>();

            ConfigurationContext.Current.CacheManager = new HttpCacheManager();

            if(setup != null)
                ConfigurationContext.Setup(setup);

            // DbContext connectionstring
            ConfigurationContext.Current.DbContextConnectionString = ConfigurationManager.ConnectionStrings[ConfigurationContext.Current.Connection].ConnectionString;

            var synchronizer = new ResourceSynchronizer();
            synchronizer.DiscoverAndRegister();

            // set model metadata providers
            if(ConfigurationContext.Current.ModelMetadataProviders.ReplaceProviders)
            {
                // set current provider
                if(ModelMetadataProviders.Current == null)
                {
                    if(ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
                        ModelMetadataProviders.Current = new CachedLocalizedMetadataProvider();
                    else
                        ModelMetadataProviders.Current = new LocalizedMetadataProvider();
                }
                else
                {
                    if(ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
                        ModelMetadataProviders.Current = new CompositeModelMetadataProvider<CachedLocalizedMetadataProvider>(ModelMetadataProviders.Current);
                    else
                        ModelMetadataProviders.Current = new CompositeModelMetadataProvider<LocalizedMetadataProvider>(ModelMetadataProviders.Current);
                }

                for(var i = 0; i < ModelValidatorProviders.Providers.Count; i++)
                {
                    var provider = ModelValidatorProviders.Providers[i];
                    if(!(provider is DataAnnotationsModelValidatorProvider))
                        continue;

                    ModelValidatorProviders.Providers.RemoveAt(i);
                    ModelValidatorProviders.Providers.Insert(i, new LocalizedModelValidatorProvider());
                    break;
                }
            }

            // in cases when there has been already a call to LoclaizationProvider.Current (some static weird things)
            // and only then setup configuration is ran - here we need to reset instance once again with new settings
            LocalizationProvider.Initialize();

            return builder;
        }
    }
}
