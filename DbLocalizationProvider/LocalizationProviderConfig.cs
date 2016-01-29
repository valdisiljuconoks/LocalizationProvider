using System;

namespace DbLocalizationProvider
{
    public class LocalizationProviderConfig
    {
        public static void Setup(Action<ConfigurationContext> action = null)
        {
            if (action == null)
            {
                return;
            }

            var context = ConfigurationContext.Current;
            action(context);
        }
    }
}
