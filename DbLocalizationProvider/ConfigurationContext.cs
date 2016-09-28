using System;
using System.Globalization;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     Context to configure various localization provider features and behavior
    /// </summary>
    public class ConfigurationContext
    {
        private CultureInfo _defaultResourceCulture;

        public ConfigurationContext()
        {
            ModelMetadataProviders = new ModelMetadataProvidersConfiguration();
        }

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

        [Obsolete("In next version this will be moved under `ModelMetadataProviders` property")]
        public bool ReplaceModelMetadataProviders
        {
            get { return ModelMetadataProviders.ReplaceProviders; }
            set { ModelMetadataProviders.ReplaceProviders = value; }
        }

        [Obsolete("In next version this will be moved under `ModelMetadataProviders` property")]
        public bool UseCachedModelMetadataProviders
        {
            get { return ModelMetadataProviders.UseCachedProviders; }
            set { ModelMetadataProviders.UseCachedProviders = value; }
        }

        public ModelMetadataProvidersConfiguration ModelMetadataProviders { get; set; }

        [Obsolete("In next version this will be moved under `ModelMetadataProviders` property")]
        public Func<bool> EnableLegacyMode
        {
            get { return ModelMetadataProviders.EnableLegacyMode; }
            set { ModelMetadataProviders.EnableLegacyMode = value; }
        }

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

        /// <summary>
        ///     Gets or sets the name of the connection.
        /// </summary>
        /// <value>
        ///     The name of the connection.
        /// </value>
        public string ConnectionName { get; set; } = "EPiServerDB";

        public TypeFactory TypeFactory { get; } = new TypeFactory();

        public ICacheManager CacheManager { get; set; } = new HttpCacheManager();

        public bool EnableInvariantCultureFallback { get; set; } = false;

        public static void Setup(Action<ConfigurationContext> configCallback)
        {
            configCallback?.Invoke(Current);
        }

        public const string CultureForTranslationsFromCode = "";
    }
}
