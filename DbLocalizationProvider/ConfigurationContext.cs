using System;
using System.Globalization;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     Context to configure various localization provider features and behavior
    /// </summary>
    public class ConfigurationContext
    {
        private CultureInfo _defaultResourceCulture;

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
        ///     Gets or sets a value to replace ModelMetadataProvider to use new db localization system.
        /// </summary>
        /// <value>
        ///     <c>true</c> if ModelMetadataProvider should be replaced; otherwise, <c>false</c>.
        /// </value>
        public bool ReplaceModelMetadataProviders { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value to use cached version of ModelMetadataProvider.
        /// </summary>
        /// <value>
        ///     <c>true</c> if cached ModelMetadataProvider should be used; otherwise, <c>false</c>.
        /// </value>
        public bool UseCachedModelMetadataProviders { get; set; } = false;

        /// <summary>
        ///     Gets or sets callback to call in order to enable ir disable legacy mode.
        ///     Legacy mode will ensure that if resource value starts with "/" symbol ModelMetadataProvider will try to look for
        ///     this XPath resource in localization provider collection once again.
        ///     This will make it possible to continue use *old* resource keys:
        ///     [DisplayName("/xpath/to/some/resource")]
        /// </summary>
        /// <value>
        ///     Return <c>true</c> to enable legacy mode translations.
        /// </value>
        public Func<bool> EnableLegacyMode { get; set; } = () => false;

        /// <summary>
        ///     Gets or sets the default resource culture to register translations for newly discovered resources.
        /// </summary>
        /// <value>
        ///     The default resource culture for translations.
        /// </value>
        public CultureInfo DefaultResourceCulture
        {
            get { return _defaultResourceCulture; }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _defaultResourceCulture = value;
            }
        }

        public static ConfigurationContext Current { get; } = new ConfigurationContext();

        /// <summary>
        ///     Gets or sets a value indicating whether Cache should be populated during startup.
        /// </summary>
        /// <value>
        ///     <c>true</c> if cache should be populated; otherwise, <c>false</c>.
        /// </value>
        public bool PopulateCacheOnStartup { get; set; } = true;

        public static void Setup(Action<ConfigurationContext> configCallback)
        {
            configCallback?.Invoke(Current);
        }
    }
}
