using System;
using Owin;

namespace DbLocalizationProvider.AdminUI
{
    public static class AppBuilderExtensions
    {
        public static void UseDbLocalizationProviderAdminUI(this IAppBuilder builder, Action<UiConfigurationContext> setup = null)
        {
            setup?.Invoke(UiConfigurationContext.Current);
        }
    }
}
