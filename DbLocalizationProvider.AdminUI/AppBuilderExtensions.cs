using System;
using DbLocalizationProvider.AdminUI.Queries;
using Owin;

namespace DbLocalizationProvider.AdminUI
{
    public static class AppBuilderExtensions
    {
        public static void UseDbLocalizationProviderAdminUI(this IAppBuilder builder, Action<UiConfigurationContext> setup = null)
        {
            // set default implementations
            ConfigurationContext.Current.TypeFactory.ForQuery<GetAvailableLanguages.Query>().SetHandler<GetAvailableLanguages.Handler>();

            setup?.Invoke(UiConfigurationContext.Current);
        }
    }
}
