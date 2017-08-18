using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Export;
using DbLocalizationProvider.Import;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     Context to configure various localization provider features and behavior
    /// </summary>
    public class ConfigurationContext
    {
        public const string CultureForTranslationsFromCode = "";
        private CultureInfo _defaultResourceCulture;
        private readonly BaseCacheManager _cacheManager = new BaseCacheManager();

        private ConfigurationContext()
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
            get => ModelMetadataProviders.ReplaceProviders;
            set => ModelMetadataProviders.ReplaceProviders = value;
        }

        [Obsolete("In next version this will be moved under `ModelMetadataProviders` property")]
        public bool UseCachedModelMetadataProviders
        {
            get => ModelMetadataProviders.UseCachedProviders;
            set => ModelMetadataProviders.UseCachedProviders = value;
        }

        public ModelMetadataProvidersConfiguration ModelMetadataProviders { get; set; }

        [Obsolete("In next version this will be moved under `ModelMetadataProviders` property")]
        public Func<bool> EnableLegacyMode
        {
            get => ModelMetadataProviders.EnableLegacyMode;
            set => ModelMetadataProviders.EnableLegacyMode = value;
        }

        /// <summary>
        ///     Gets or sets the default resource culture to register translations for newly discovered resources.
        /// </summary>
        /// <value>
        ///     The default resource culture for translations.
        /// </value>
        public CultureInfo DefaultResourceCulture
        {
            get => _defaultResourceCulture;
            set => _defaultResourceCulture = value ?? throw new ArgumentNullException(nameof(value));
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

        public ICacheManager CacheManager
        {
            get => _cacheManager;
            set
            {
                if(value != null)
                    _cacheManager.SetInnerManager(value);
            }
        }

        public bool EnableInvariantCultureFallback { get; set; } = false;

        public Func<Assembly, bool> AssemblyScanningFilter { get; set; } =
            a => !a.FullName.StartsWith("Microsoft")
                 && !a.FullName.StartsWith("mscorlib")
                 && !a.FullName.StartsWith("System")
                 && !a.FullName.StartsWith("EPiServer")
                 && !a.FullName.StartsWith("EntityFramework")
                 && !a.FullName.StartsWith("Newtonsoft");

        public bool DiagnosticsEnabled { get; set; } = false;

        public ICollection<CustomAttributeDescriptor> CustomAttributes { get; set; } = new List<CustomAttributeDescriptor>();

        public ICollection<ForeignResourceDescriptor> ForeignResources { get; set; } = new List<ForeignResourceDescriptor>();

        public ExportSettings Export { get; set; } = new ExportSettings();

        public ImportSettings Import { get; set; } = new ImportSettings();

        public static void Setup(Action<ConfigurationContext> configCallback)
        {
            configCallback?.Invoke(Current);
        }
    }
}
