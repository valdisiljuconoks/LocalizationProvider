using System;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Owin;

namespace DbLocalizationProvider
{
    public static class AppBuilderExtensions
    {
        public static void UseDbLocalizationProvider(this IAppBuilder builder, Action<ConfigurationContext> setup = null)
        {
            // setup default implementations
            ConfigurationContext.Current.TypeFactory.ForQuery<AvailableLanguages.Query>().SetHandler<AvailableLanguages.Handler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslation.Handler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResources.Handler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllTranslations.Query>().SetHandler<GetAllTranslations.Handler>();

            ConfigurationContext.Current.TypeFactory.ForCommand<CreateNewResource.Command>().SetHandler<CreateNewResource.Handler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResource.Handler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslation.Handler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<ClearCache.Command>().SetHandler<ClearCache.Handler>();

            if(setup != null)
                ConfigurationContext.Setup(setup);

            var synchronizer = new ResourceSynchronizer();
            synchronizer.DiscoverAndRegister();
        }
    }
}
