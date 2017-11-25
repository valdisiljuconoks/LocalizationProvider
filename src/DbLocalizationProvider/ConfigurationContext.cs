// Copyright © 2017 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Export;
using DbLocalizationProvider.Import;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     Context to configure various localization provider features and behavior
    /// </summary>
    public class ConfigurationContext
    {
        public const string CultureForTranslationsFromCode = "";
        private readonly BaseCacheManager _cacheManager = new BaseCacheManager();
        private CultureInfo _defaultResourceCulture;

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
        public bool ReplaceModelMetadataProviders { get => ModelMetadataProviders.ReplaceProviders; set => ModelMetadataProviders.ReplaceProviders = value; }

        [Obsolete("In next version this will be moved under `ModelMetadataProviders` property")]
        public bool UseCachedModelMetadataProviders { get => ModelMetadataProviders.UseCachedProviders; set => ModelMetadataProviders.UseCachedProviders = value; }

        public ModelMetadataProvidersConfiguration ModelMetadataProviders { get; set; }

        [Obsolete("In next version this will be moved under `ModelMetadataProviders` property")]
        public Func<bool> EnableLegacyMode { get => ModelMetadataProviders.EnableLegacyMode; set => ModelMetadataProviders.EnableLegacyMode = value; }

        /// <summary>
        ///     Gets or sets the default resource culture to register translations for newly discovered resources.
        /// </summary>
        /// <value>
        ///     The default resource culture for translations.
        /// </value>
        public CultureInfo DefaultResourceCulture { get => _defaultResourceCulture;
            set
            {
                if(value != null && value.Equals(CultureInfo.InvariantCulture))
                    throw new InvalidOperationException("InvariantCulture cannot be used as default culture.");

                _defaultResourceCulture = value ?? throw new ArgumentNullException(nameof(value));
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

        public List<IResourceTypeScanner> TypeScanners { get; } = new List<IResourceTypeScanner>
                                                                  {
                                                                      new LocalizedModelTypeScanner(),
                                                                      new LocalizedResourceTypeScanner(),
                                                                      new LocalizedEnumTypeScanner(),
                                                                      new LocalizedForeignResourceTypeScanner()
                                                                  };

        public static void Setup(Action<ConfigurationContext> configCallback)
        {
            configCallback?.Invoke(Current);
        }
    }
}
