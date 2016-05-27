using System;
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

            var synchronizer = new ResourceSynchronizer();
            synchronizer.DiscoverAndRegister();
        }
    }
}
