// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AspNetCore.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Does config for <see cref="MvcOptions.ModelMetadataDetailsProviders" />.
/// </summary>
public class ConfigureModelMetadataDetailsProviders : IConfigureOptions<MvcOptions>
{
    private readonly IOptions<ConfigurationContext> _configurationContext;
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly ILocalizationProvider _localizationProvider;

    /// <summary>
    /// Creates new instance of the class (what else?).
    /// </summary>
    /// <param name="localizationProvider">localization provider.</param>
    /// <param name="keyBuilder">Resource key builder.</param>
    /// <param name="configurationContext">Context of the library configuration.</param>
    public ConfigureModelMetadataDetailsProviders(
        ILocalizationProvider localizationProvider,
        ResourceKeyBuilder keyBuilder,
        IOptions<ConfigurationContext> configurationContext)
    {
        _localizationProvider = localizationProvider;
        _keyBuilder = keyBuilder;
        _configurationContext = configurationContext;
    }

    /// <inheritdoc />
    public void Configure(MvcOptions options)
    {
        options.ModelMetadataDetailsProviders.Add(
            new LocalizedDisplayMetadataProvider(
                new ModelMetadataLocalizationHelper(_localizationProvider, _keyBuilder, _configurationContext),
                _configurationContext));
    }
}
