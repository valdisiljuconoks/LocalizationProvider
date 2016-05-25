using Owin;

namespace DbLocalizationProvider.Sync
{
    public static class AppBuilderExtensions
    {
        public static void UseDbLocalizationProvider(this IAppBuilder builder)
        {
            var synchronizator = new ResourceSynchronizer();
            synchronizator.DiscoverAndRegister();
        }
    }
}
