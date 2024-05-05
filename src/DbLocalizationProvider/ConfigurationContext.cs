// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Export;
using DbLocalizationProvider.Import;
using DbLocalizationProvider.Logging;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider;

/// <summary>
/// Context to configure various localization provider features and behavior
/// </summary>
public class ConfigurationContext
{
    internal IServiceCollection Services { get; }

    /// <summary>
    /// Value indicating default culture for resources registered from code.
    /// </summary>
    public const string CultureForTranslationsFromCode = "";

    internal BaseCacheManager _baseCacheManager = new(new InMemoryCache());

    internal FallbackLanguagesCollection _fallbackCollection = new();

    /// <summary>
    /// Creates new instance of configuration settings.
    /// </summary>
    public ConfigurationContext()
    {
        TypeFactory = new TypeFactory(new OptionsWrapper<ConfigurationContext>(this));
        FallbackLanguages = _fallbackCollection.GetFallbackLanguages("default");
}

    /// <summary>
    /// Creates new instance of configuration settings class.
    /// </summary>
    /// <param name="services">Services collection (if needed by any provider configuration pipeline).</param>
    public ConfigurationContext(IServiceCollection services) : this()
    {
        Services = services;
    }

    /// <summary>
    /// Gets or sets the callback for enabling or disabling localization. If this returns <c>false</c> - resource key will
    /// be returned. Default <c>true</c>.
    /// </summary>
    /// <value>
    /// <c>true</c> to enable localization; otherwise - <c>false</c>.
    /// </value>
    public Func<bool> EnableLocalization { get; set; } = () => true;

    /// <summary>
    /// Gets or sets callback to call in order to enable ir disable legacy mode.
    /// Legacy mode will ensure that if resource value starts with "/" symbol ModelMetadataProvider will try to look for
    /// this XPath resource in localization provider collection once again.
    /// This will make it possible to continue use *old* resource keys:
    /// [DisplayName("/xpath/to/some/resource")]
    /// Default <c>false</c>.
    /// </summary>
    /// <value>
    /// Return <c>true</c> to enable legacy mode translations.
    /// </value>
    public Func<bool> EnableLegacyMode { get; set; } = () => false;

    /// <summary>
    /// Gets or sets the flag to control localized models discovery and registration during app startup or whenever you initialize provider.
    /// Default <c>true</c>.
    /// </summary>
    /// <value>
    /// Discovers and registers localized models.
    /// </value>
    public bool DiscoverAndRegisterResources { get; set; } = true;

    /// <summary>
    /// Forces type scanner to load all referenced assemblies. When enabled, scanner is not relying on current
    /// AppDomain.GetAssemblies but checks referenced assemblies recursively.
    /// Default <c>false</c>.
    /// </summary>
    /// <value>
    /// By default this feature is disabled.
    /// </value>
    public bool ScanAllAssemblies { get; set; } = false;

    /// <summary>
    /// Settings for model metadata providers.
    /// </summary>
    public ModelMetadataProvidersConfiguration ModelMetadataProviders { get; set; } =
        new();

    /// <summary>
    /// Gets or sets the default resource culture to register translations for newly discovered resources.
    /// </summary>
    /// <value>
    /// The default resource culture for translations.
    /// </value>
    public CultureInfo DefaultResourceCulture { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether cache should be populated during startup (default = true).
    /// Default <c>true</c>.
    /// </summary>
    /// <value>
    /// <c>true</c> if cache should be populated; otherwise, <c>false</c>.
    /// </value>
    public bool PopulateCacheOnStartup { get; set; } = true;

    /// <summary>
    /// Returns type factory used internally for creating new services or handlers for commands.
    /// </summary>
    public TypeFactory TypeFactory { get; internal set; }

    /// <summary>
    /// Gets or sets callback whether lookup resource by requested key.
    /// Use with caution. This is optimization workaround for the cases when you need to filter out and allow some of the resources to pass-through
    /// for <see cref="Queries.GetTranslation.Query" /> query.
    /// </summary>
    /// <remarks>Return <c>true</c> if you want to continue translation lookup for given resource key</remarks>
    public Func<string, bool> ResourceLookupFilter { internal get; set; }

    /// <summary>
    /// Gets or sets cache manager used to store resources and translations
    /// </summary>
    public ICacheManager CacheManager
    {
        get => _baseCacheManager;
        set
        {
            if (value != null)
            {
                _baseCacheManager.SetInnerManager(value);
            }
        }
    }

    private void CopyCacheManager(BaseCacheManager cacheManager)
    {
        _baseCacheManager = cacheManager;
    }

    /// <summary>
    /// Gets or sets flag to enable or disable invariant culture fallback (to use resource values discovered and registered from code).
    /// Default <c>false</c>.
    /// </summary>
    public bool EnableInvariantCultureFallback { get; set; } = false;

    /// <summary>
    /// Gets or sets filter to apply for assembly list in application for reducing time spent during scanning.
    /// </summary>
    public Func<Assembly, bool> AssemblyScanningFilter { get; set; } =
        a => a.FullName != null
             && !a.IsDynamic
             && !a.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)
             && !a.FullName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase)
             && !a.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase)
             && !a.FullName.StartsWith("EPiServer", StringComparison.OrdinalIgnoreCase)
             && !a.FullName.StartsWith("EntityFramework", StringComparison.OrdinalIgnoreCase)
             && !a.FullName.StartsWith("Newtonsoft", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets value enabling or disabling diagnostics for localization provider (e.g. missing keys will be written
    /// to log file).
    /// Default <c>false</c>.
    /// </summary>
    public bool DiagnosticsEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets list of custom attributes that should be discovered and registered during startup scanning.
    /// </summary>
    public ICollection<CustomAttributeDescriptor> CustomAttributes { get; set; } = new List<CustomAttributeDescriptor>();

    /// <summary>
    /// Gets or sets collection of foreign resources. Foreign resource descriptors are used to include classes without
    /// <c>[LocalizedResource]</c> or <c>[LocalizedModel]</c> attributes.
    /// </summary>
    public ICollection<ForeignResourceDescriptor> ForeignResources { get; set; } = new List<ForeignResourceDescriptor>();

    /// <summary>
    /// Gets or sets settings used for export of the resources.
    /// </summary>
    public ExportSettings Export { get; set; } = new();

    /// <summary>
    /// Gets or sets settings to be used during resource import.
    /// </summary>
    public ImportSettings Import { get; set; } = new();

    /// <summary>
    /// Gets list of all known type scanners.
    /// </summary>
    public List<IResourceTypeScanner> TypeScanners { get; } = new();

    /// <summary>
    /// This is your last chance to lookup translations in other languages if there is none for the requested one.
    /// </summary>
    public FallbackLanguages FallbackLanguages { get; internal set; }

    /// <summary>
    /// Gets or sets the logger to be used by the localization provider library. Depending on runtime platform specific implementations may use
    /// this interface to add adapter for their logging infra.
    /// </summary>
    public ILogger Logger { get; set; } = new NullLogger();

    /// <summary>
    /// If you are looking for a way to allow other characters in resource key name - this is the property to set.
    /// </summary>
    public Regex ResourceKeyNameFilter { get; set; } = new("^[.@+\\\"\\=\\/\\[\\]a-zA-Z0-9]+$");

    /// <summary>
    /// You can set provider that would return manual resources to sync.
    /// </summary>
    public ManualResourceProviderCollection ManualResourceProviders { get; } = new();

    /// <summary>
    /// Wanna chill a bit? The use this flexible and relaxed refactored resource sync mode.
    /// By enabling this you are telling sync process not to panic if there is already existing refactored resources in target db.
    /// This easily can happen if you switch between two branches from which one of them contains refactored code already.
    /// Default <c>false</c>.
    /// </summary>
    public bool FlexibleRefactoringMode { get; set; } = false;

    internal void CopyFrom(ConfigurationContext ctx)
    {
        if (ctx == null)
        {
            throw new ArgumentNullException(nameof(ctx));
        }

        CopyCacheManager(ctx._baseCacheManager);

        EnableLocalization = ctx.EnableLocalization;
        EnableLegacyMode = ctx.EnableLegacyMode;
        DiscoverAndRegisterResources = ctx.DiscoverAndRegisterResources;
        ScanAllAssemblies = ctx.ScanAllAssemblies;
        DefaultResourceCulture = ctx.DefaultResourceCulture;
        ResourceLookupFilter = ctx.ResourceLookupFilter;
        EnableInvariantCultureFallback = ctx.EnableInvariantCultureFallback;
        DiagnosticsEnabled = ctx.DiagnosticsEnabled;
        CustomAttributes = ctx.CustomAttributes;
        ForeignResources = ctx.ForeignResources;
        Export = ctx.Export;
        Import = ctx.Import;
        _fallbackCollection = ctx._fallbackCollection;
        FallbackLanguages = ctx.FallbackLanguages;
        Logger = ctx.Logger;
        ResourceKeyNameFilter = ctx.ResourceKeyNameFilter;
        FlexibleRefactoringMode = ctx.FlexibleRefactoringMode;
    }
}
