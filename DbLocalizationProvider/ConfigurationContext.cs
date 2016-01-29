using System;

namespace DbLocalizationProvider
{
    public class ConfigurationContext
    {
        public Func<bool> DisableLocalizationCallback { get; set; } = () => false;

        public static ConfigurationContext Current { get; } = new ConfigurationContext();
    }
}
