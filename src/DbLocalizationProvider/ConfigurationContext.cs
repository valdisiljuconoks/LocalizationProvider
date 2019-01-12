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
        /// <summary>
        /// Value indicating default culture for resources registered from code.
        /// </summary>
        public const string CultureForTranslationsFromCode = "";

        internal readonly BaseCacheManager BaseCacheManager = new BaseCacheManager(new InMemoryCache());
        internal string DbContextConnectionString;

        private ConfigurationContext()
        {
            ModelMetadataProviders = new ModelMetadataProvidersConfiguration();
        }

        /// <summary>
        ///     Gets or sets the callback for enabling or disabling localization. If this returns <c>false</c> - resource key will be returned.
        /// </summary>
        /// <value>
        ///     <c>true</c> to enable localization; otherwise - <c>false</c>.
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
        /// Settings for model metadata providers.
        /// </summary>
        public ModelMetadataProvidersConfiguration ModelMetadataProviders { get; set; }

        /// <summary>
        ///     Gets or sets the default resource culture to register translations for newly discovered resources.
        /// </summary>
        /// <value>
        ///     The default resource culture for translations.
        /// </value>
        public CultureInfo DefaultResourceCulture { get; set; }

        /// <summary>
        /// Returns current instance of the configuration context.
        /// </summary>
        public static ConfigurationContext Current { get; } = new ConfigurationContext();

        /// <summary>
        ///     Gets or sets a value indicating whether cache should be populated during startup.
        /// </summary>
        /// <value>
        ///     <c>true</c> if cache should be populated; otherwise, <c>false</c>.
        /// </value>
        public bool PopulateCacheOnStartup { get; set; } = true;

        /// <summary>
        ///     Gets or sets the name of the database connection.
        /// </summary>
        /// <value>
        ///     The name of the connection.
        /// </value>
        public string Connection { get; set; } = "DefaultConnection";

        /// <summary>
        /// Returns type factory used internally for creating new services or handlers for commands.
        /// </summary>
        public TypeFactory TypeFactory { get; } = new TypeFactory();

        /// <summary>
        /// Gets or sets cache manager used to store resources and translations
        /// </summary>
        public ICacheManager CacheManager
        {
            get => BaseCacheManager;
            set
            {
                if(value != null)
                    BaseCacheManager.SetInnerManager(value);
            }
        }

        /// <summary>
        /// Gets or sets flag to enable or disable invariant culture fallback (to use resource values discovered & registered from code).
        /// </summary>
        public bool EnableInvariantCultureFallback { get; set; } = false;

        /// <summary>
        /// Gets or sets filter to apply for assembly list in application for reducing time spent during scanning.
        /// </summary>
        public Func<Assembly, bool> AssemblyScanningFilter { get; set; } =
            a => !a.FullName.StartsWith("Microsoft")
                 && !a.FullName.StartsWith("mscorlib")
                 && !a.FullName.StartsWith("System")
                 && !a.FullName.StartsWith("EPiServer")
                 && !a.FullName.StartsWith("EntityFramework")
                 && !a.FullName.StartsWith("Newtonsoft");

        /// <summary>
        /// Gets or sets value enabling or disabling diagnostics for localization provider (e.g. missing keys will be written to log file).
        /// </summary>
        public bool DiagnosticsEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets list of custom attributes that should be discovered and registered during startup scanning.
        /// </summary>
        public ICollection<CustomAttributeDescriptor> CustomAttributes { get; set; } = new List<CustomAttributeDescriptor>();

        /// <summary>
        /// Gets or sets collection of foreign resources. Foreign resource descriptors are used to include classes without <c>[LocalizedResource]</c> or <c>[LocalizedModel]</c> attributtes.
        /// </summary>
        public ICollection<ForeignResourceDescriptor> ForeignResources { get; set; } = new List<ForeignResourceDescriptor>();

        /// <summary>
        /// Gets or sets settings used for export of the resources.
        /// </summary>
        public ExportSettings Export { get; set; } = new ExportSettings();

        /// <summary>
        /// Gets or sets settings to be used during resource import.
        /// </summary>
        public ImportSettings Import { get; set; } = new ImportSettings();

        /// <summary>
        /// Gets list of all known type scanners.
        /// </summary>
        public List<IResourceTypeScanner> TypeScanners { get; } = new List<IResourceTypeScanner>
                                                                  {
                                                                      new LocalizedModelTypeScanner(),
                                                                      new LocalizedResourceTypeScanner(),
                                                                      new LocalizedEnumTypeScanner(),
                                                                      new LocalizedForeignResourceTypeScanner()
                                                                  };

        /// <summary>
        /// Method to initialize and configure localization provider.
        /// </summary>
        /// <param name="configCallback">Configuration setup action.</param>
        public static void Setup(Action<ConfigurationContext> configCallback)
        {
            configCallback?.Invoke(Current);
        }
    }
}
