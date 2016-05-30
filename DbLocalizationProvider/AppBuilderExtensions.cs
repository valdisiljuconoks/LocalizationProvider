using System;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Sync;
using Owin;

namespace DbLocalizationProvider
{
    public static class AppBuilderExtensions
    {
        public static void UseDbLocalizationProvider(this IAppBuilder builder, Action<ConfigurationContext> setup = null)
        {
            if(setup != null)
                ConfigurationContext.Setup(setup);

            ConfigurationContext.Current.Repository = new CachedLocalizationResourceRepository(new LocalizationResourceRepository(), new HttpCacheManager());

            var synchronizer = new ResourceSynchronizer();
            synchronizer.DiscoverAndRegister();
        }
    }
}
