using System;

namespace DbLocalizationProvider
{
    public class ConfigurationContext
    {
        /// <summary>
        ///     Gets or sets the disable localization callback.
        /// </summary>
        /// <value>
        ///     The disable localization callback.
        /// </value>
        public Func<bool> EnableLocalization { get; set; } = () => true;

        /// <summary>
        ///     Gets or sets the flag to control localized models discovery and registration during app startup.
        /// </summary>
        /// <value>
        ///     Discovers and registers localized models.
        /// </value>
        public bool DiscoverAndRegisterResources { get; set; } = true;

        /// <summary>
        /// Gets or sets a value to replace ModelMetadataProvider to use new db localization system.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ModelMetadataProvider should be replaced; otherwise, <c>false</c>.
        /// </value>
        public bool ReplaceModelMetadataProviders { get; set; } = true;

        public static ConfigurationContext Current { get; } = new ConfigurationContext();

        public static void Setup(Action<ConfigurationContext> configCallback)
        {
            configCallback?.Invoke(Current);
        }
    }
}
