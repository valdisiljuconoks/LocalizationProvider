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
using System.Web.Mvc;
using DbLocalizationProvider.AspNet.Cache;
using DbLocalizationProvider.AspNet.Commands;
using DbLocalizationProvider.AspNet.Queries;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.DataAnnotations;
using DbLocalizationProvider.EPiServer.Queries;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.EPiServer
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class DbLocalizationProviderInitializationModule : IConfigurableModule
    {
        private ServiceConfigurationContext _context;
        private bool _eventHandlerAttached;

        public void Initialize(InitializationEngine context)
        {
            if(_eventHandlerAttached)
                return;

            context.InitComplete += DiscoverAndRegister;
            _eventHandlerAttached = true;
        }

        public void Uninitialize(InitializationEngine context)
        {
            context.InitComplete -= DiscoverAndRegister;
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // we need to capture original context in order to replace ModelMetaDataProvider later if needed
            _context = context;
        }

        private void DiscoverAndRegister(object sender, EventArgs eventArgs)
        {
            ConfigurationContext.Setup(ctx =>
                                       {
                                           ctx.ConnectionName = "EPiServerDB";
                                           ctx.CacheManager = new EPiServerCacheManager();

                                           ctx.TypeScanners.Insert(0, new LocalizedCategoryScanner());

                                           ctx.TypeFactory.ForQuery<AvailableLanguages.Query>().SetHandler<EPiServerAvailableLanguages.Handler>();
                                           ctx.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<EPiServerGetTranslation.Handler>();

                                           ctx.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResourcesHandler>();
                                           ctx.TypeFactory.ForQuery<GetAllTranslations.Query>().SetHandler<GetAllTranslationsHandler>();

                                           ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<EPiServerDetermineDefaultCulture.Handler>();

                                           ctx.TypeFactory.ForCommand<CreateNewResource.Command>().SetHandler<CreateNewResourceHandler>();
                                           ctx.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResourceHandler>();
                                           ctx.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslationHandler>();
                                           ctx.TypeFactory.ForCommand<ClearCache.Command>().SetHandler<ClearCacheHandler>();
                                       });

            var synchronizer = new ResourceSynchronizer();
            synchronizer.DiscoverAndRegister();

            if(!ConfigurationContext.Current.ModelMetadataProviders.ReplaceProviders)
                return;

            if(!_context.Services.Contains(typeof(ModelMetadataProvider)))
            {
                // set new provider
                if(ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
                    _context.Services.AddSingleton<ModelMetadataProvider, CachedLocalizedMetadataProvider>();
                else
                    _context.Services.AddSingleton<ModelMetadataProvider, LocalizedMetadataProvider>();
            }
            else
            {
                var currentProvider = ServiceLocator.Current.GetInstance<ModelMetadataProvider>();

                // decorate existing provider
                if(ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
                    _context.Services.AddSingleton<ModelMetadataProvider>(new CompositeModelMetadataProvider<CachedLocalizedMetadataProvider>(currentProvider));
                else
                    _context.Services.AddSingleton<ModelMetadataProvider>(new CompositeModelMetadataProvider<LocalizedMetadataProvider>(currentProvider));
            }

            for (var i = 0; i < ModelValidatorProviders.Providers.Count; i++)
            {
                var provider = ModelValidatorProviders.Providers[i];
                if(!(provider is DataAnnotationsModelValidatorProvider))
                {
                    continue;
                }

                ModelValidatorProviders.Providers.RemoveAt(i);
                ModelValidatorProviders.Providers.Insert(i, new LocalizedModelValidatorProvider());
                break;
            }
        }
    }
}
