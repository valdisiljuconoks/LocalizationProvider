using System;
using DbLocalizationProvider.Cache;
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
            ConfigurationContext.Current.Repository = new CachedLocalizationResourceRepository(new LocalizationResourceRepository(), new HttpCacheManager());
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslation.Handler>();

            if(setup != null)
                ConfigurationContext.Setup(setup);

            var synchronizer = new ResourceSynchronizer();
            synchronizer.DiscoverAndRegister();
        }
    }
}
