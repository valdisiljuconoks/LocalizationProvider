using System;
using System.Web.Mvc;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.DataAnnotations;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Owin;

namespace DbLocalizationProvider
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseDbLocalizationProvider(this IAppBuilder builder, Action<ConfigurationContext> setup = null)
        {
            // setup default implementations
            ConfigurationContext.Current.TypeFactory.ForQuery<AvailableLanguages.Query>().SetHandler<AvailableLanguages.Handler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslation.Handler>();

            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResources.Handler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllTranslations.Query>().SetHandler<GetAllTranslations.Handler>();

            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            ConfigurationContext.Current.TypeFactory.ForCommand<CreateNewResource.Command>().SetHandler<CreateNewResource.Handler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResource.Handler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslation.Handler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<ClearCache.Command>().SetHandler<ClearCache.Handler>();

            if(setup != null)
                ConfigurationContext.Setup(setup);

            var synchronizer = new ResourceSynchronizer();
            synchronizer.DiscoverAndRegister();

            // set model metadata providers
            if(ConfigurationContext.Current.ModelMetadataProviders.ReplaceProviders)
            {
                // set current provider
                if(ModelMetadataProviders.Current == null)
                {
                    if(ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
                    {
                        ModelMetadataProviders.Current = new CachedLocalizedMetadataProvider();
                    }
                    else
                    {
                        ModelMetadataProviders.Current = new LocalizedMetadataProvider();
                    }
                }
                else
                {
                    if (ConfigurationContext.Current.ModelMetadataProviders.UseCachedProviders)
                    {
                        ModelMetadataProviders.Current = new CompositeModelMetadataProvider<CachedLocalizedMetadataProvider>(ModelMetadataProviders.Current);
                    }
                    else
                    {
                        ModelMetadataProviders.Current = new CompositeModelMetadataProvider<LocalizedMetadataProvider>(ModelMetadataProviders.Current);
                    }
                }

                for (var i = 0; i < ModelValidatorProviders.Providers.Count; i++)
                {
                    var provider = ModelValidatorProviders.Providers[i];
                    if (!(provider is DataAnnotationsModelValidatorProvider))
                    {
                        continue;
                    }

                    ModelValidatorProviders.Providers.RemoveAt(i);
                    ModelValidatorProviders.Providers.Insert(i, new LocalizedModelValidatorProvider());
                    break;
                }
            }

            return builder;
        }
    }
}
